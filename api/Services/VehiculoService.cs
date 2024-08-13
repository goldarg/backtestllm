using System.Text;
using System.Text.Json;
using api.Connected_Services;
using api.DataAccess;
using api.Exceptions;
using api.Models.DTO;
using api.Models.DTO.Contrato;
using api.Models.DTO.Operaciones;
using api.Models.DTO.Vehiculo;
using api.Models.Entities;
using Newtonsoft.Json.Linq;

namespace api.Services;

public class VehiculoService(
    IHttpClientFactory _httpClientFactory,
    IRdaUnitOfWork _unitOfWork,
    CRMService _crmService,
    IUserIdentityService _identityService,
    IActividadUsuarioService _actividadUsuarioService,
    IContratoService _contratoService
) : IVehiculoService
{
    public async Task<string?> AsignarVehiculo(AsignarVehiculoDto asignarVehiculoDto)
    {
        var userRepository = _unitOfWork.GetRepository<User>();

        var httpClient = _httpClientFactory.CreateClient("CrmHttpClient");

        ValidateAsignarVehiculo(asignarVehiculoDto, httpClient);

        if (asignarVehiculoDto.usuarioId == null)
        {
            asignarVehiculoDto.usuarioId = userRepository
                .GetAll()
                .Where(x => x.nombre == "Sin" && x.apellido == "Asignar")
                .First()
                .idCRM;
        }

        //Busco el conductor que ahora voy a sacar, para agregarle el log mas adelante
        var uri = new StringBuilder(
            $"crm/v2/{asignarVehiculoDto.tipoContrato}/{asignarVehiculoDto.idContratoInterno}?fields=Conductor"
        );
        var json = await _crmService.Get(uri.ToString());
        var conductorViejoCrm = JsonSerializer.Deserialize<List<BuscarConductorDto>>(json);

        var conductorViejoId =
            conductorViejoCrm.Count() > 0 ? conductorViejoCrm.First().Conductor.id : null;

        //Busco y actualizo según el tipo de contrato
        string targetModule;
        if (asignarVehiculoDto.tipoContrato == "Fleet Management")
            asignarVehiculoDto.tipoContrato = "Servicios_RDA";
        else if (asignarVehiculoDto.tipoContrato == "Renting")
            asignarVehiculoDto.tipoContrato = "Renting";
        else if (asignarVehiculoDto.tipoContrato == "Alquiler Corporativo")
            asignarVehiculoDto.tipoContrato = "Alquileres";
        else
            throw new BadRequestException("No se pudo determinar el tipo de contrato del vehículo");

        uri = new StringBuilder($"crm/v2/{asignarVehiculoDto.tipoContrato}/upsert");

        //Armo el objeto para enviar al CRM, y devuelvo la respuesta
        var jsonObject = new
        {
            data = new[]
            {
                new
                {
                    id = asignarVehiculoDto.idContratoInterno,
                    Conductor = new { id = asignarVehiculoDto.usuarioId }
                }
            }
        };

        string jsonString = JsonSerializer.Serialize(
            jsonObject,
            new JsonSerializerOptions { WriteIndented = true }
        );
        HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(uri.ToString(), content);

        _actividadUsuarioService.CrearActividadCrm(
            asignarVehiculoDto.usuarioId,
            "Asignación de vehículo " + asignarVehiculoDto.dominio
        );

        //Puede no tener conductor anterior. Si lo tiene, le agrego el log
        if (conductorViejoId != null)
        {
            _actividadUsuarioService.CrearActividadCrm(
                conductorViejoId,
                "Desasignación del vehículo" + asignarVehiculoDto.dominio
            );
        }

        return await response.Content.ReadAsStringAsync();
    }

    private async void ValidateAsignarVehiculo(
        AsignarVehiculoDto asignarVehiculoDto,
        HttpClient httpClient
    )
    {
        var usuarioDb = _unitOfWork
            .GetRepository<User>()
            .GetAll()
            .Where(x => x.idCRM == asignarVehiculoDto.usuarioId)
            .FirstOrDefault();

        if (usuarioDb == null)
            throw new BadRequestException("Error al identificar el usuario");

        if (usuarioDb.estado != "Activo")
            throw new BadRequestException(
                "El usuario debe tener estado 'Activo' para asignarle un vehículo"
            );

        var uri = new StringBuilder(
            $"crm/v2/vehiculos/{asignarVehiculoDto.vehiculoId}?fields=Estado"
        );
        var json = await _crmService.Get(uri.ToString());
        var vehiculoCrm = JsonSerializer.Deserialize<List<VehiculoEstadoDto>>(json);

        if (vehiculoCrm.Count() != 1)
            throw new BadRequestException("Error al buscar el vehículo a asignar");

        if (vehiculoCrm.First().Estado == "Inactivo")
            throw new BadRequestException("No se puede asignar un vehiculo en estado 'Inactivo'");
    }

    public async Task<List<VehiculoDto>?> GetVehiculos()
    {
        //Get a Vehiculos con los datos que necesito
        var uri = new StringBuilder(
            "crm/v2/Vehiculos?fields=id,Name,Estado,Marca_Vehiculo,Modelo,Versi_n,Chasis,Color,A_o,Medida_Cubierta,"
                + "Fecha_de_patentamiento,Compa_a_de_seguro,Franquicia,Poliza_N,Vencimiento_Matafuego,"
                + "Vencimiento_de_Ruta,Padron,Vto_Cedula_Verde,Ultimo_Odometro_KM,Fecha_siguiente_VTV,Pa_s,Tipo_cobertura"
        );
        var getVehiculosTask = _crmService.Get(uri.ToString());
        var getContratosTask = _contratoService.GetContratos();
        await Task.WhenAll(getVehiculosTask, getContratosTask);

        var json = await getVehiculosTask;
        var vehiculos = JsonSerializer.Deserialize<List<VehiculoDto>>(json);
        if (vehiculos == null)
            return [];

        var contratos = await getContratosTask;
        if (contratos == null)
            return [];

        var vehiculosDict = vehiculos.ToDictionary(v => v.Name);

        var vehiculosRespuesta = new List<VehiculoDto>();

        MapearContratosAVehiculos(vehiculosDict, contratos.ContratosRenting, vehiculosRespuesta);
        MapearContratosAVehiculos(vehiculosDict, contratos.ContratosAlquiler, vehiculosRespuesta);
        MapearContratosAVehiculos(vehiculosDict, contratos.ContratosTelemetria, vehiculosRespuesta);
        MapearContratosAVehiculos(
            vehiculosDict,
            contratos.ContratosServicioRda,
            vehiculosRespuesta
        );

        // si es conductor filtrar vehiculos
        var usuario = _identityService.GetUsuarioDb();
        if (_identityService.UsuarioPoseeRol("CONDUCTOR"))
            vehiculosRespuesta = vehiculosRespuesta
                .Where(v => v.Conductor?.id == usuario.idCRM)
                .ToList();

        return vehiculosRespuesta;
    }

    public async Task<List<OperacionesVehiculoDto>> HistorialOperaciones(
        string dominio,
        string tipoContrato
    )
    {
        var empresasDisponibles = _identityService.ListarEmpresasDelUsuario();

        var uris = new Dictionary<string, string>
        {
            {
                "Alquiler Corporativo",
                $"crm/v2/Alquileres/search?criteria=(Dominio_Alquiler.name:equals:"
                    + dominio
                    + ")&fields=Dominio_Alquiler,Contrato"
            },
            {
                "Fleet Management",
                $"crm/v2/Servicios_RDA/search?criteria=(Dominio.name:equals:"
                    + dominio
                    + ")&fields=Dominio,Contrato"
            },
            {
                "Renting",
                $"crm/v2/Renting/search?criteria=(Dominio.name:equals:"
                    + dominio
                    + ")&fields=Dominio,Nombre_del_contrato"
            }
        };

        var json = await _crmService.Get(uris[tipoContrato]);
        var contratoId = JArray
            .Parse(json)[0][tipoContrato == "Renting" ? "Nombre_del_contrato" : "Contrato"]
            .ToObject<CRMRelatedObject>()
            .id;

        var uri = new StringBuilder(
            "crm/v2/Contratos/search?criteria=(Id:equals:" + contratoId + ")&fields=Cuenta"
        );

        json = await _crmService.Get(uri.ToString());

        var contrato = JsonSerializer.Deserialize<List<ContratoDto>>(json)[0];

        if (!empresasDisponibles.Contains(contrato.Cuenta.id))
            return [];

        uri = new StringBuilder(
            "crm/v2/Purchase_Orders/search?criteria=(Vehiculo.name:equals:"
                + dominio
                + ")"
                + "&fields=id,Clasificaciones,Vehiculo,Product_Details,Vendor_Name,Turno,Status,PO_Number"
        );

        json = await _crmService.Get(uri.ToString());
        var operaciones = JsonSerializer.Deserialize<List<OperacionesResponseDto>>(json);

        var response = operaciones
            .Select(o => new OperacionesVehiculoDto
            {
                Id = o.Id,
                TipoOperacion = o.TipoOperacion,
                Detalle = o.Detalle.Any() ? o.Detalle.Select(d => d.Product.Name).ToList() : null,
                Taller = o.Taller?.name,
                FechaTurno = o.FechaTurno,
                Estado = o.Estado,
                OT = o.OT
            })
            .ToList();

        return response;
    }

    /// <summary>
    /// Inyecta información de contratos en los vehículos correspondientes.
    /// </summary>
    /// <param name="vehiculosDict">Diccionario que mapea dominios a objetos VehiculoDto.</param>
    /// <param name="contratos">Colección de contratos a procesar.</param>
    /// <param name="vehiculosRespuesta">Lista de vehículos actualizados con la información de los contratos.</param>
    /// <remarks>
    /// El método recorre la lista de contratos y, si encuentra un vehículo correspondiente en el diccionario,
    /// actualiza las propiedades de VehiculoDto con la información del contrato.
    /// </remarks>
    private void MapearContratosAVehiculos(
        Dictionary<string, VehiculoDto> vehiculosDict,
        IEnumerable<ContratoBaseDto> contratos,
        List<VehiculoDto> vehiculosRespuesta
    )
    {
        foreach (var c in contratos)
        {
            if (
                !string.IsNullOrEmpty(c?.Dominio?.name)
                && vehiculosDict.TryGetValue(c.Dominio.name, out var v)
            )
            {
                v.Conductor = c.Conductor;
                v.Contrato = c.ContratoPadre;
                v.plazoContrato = c.ContratoMarco.PlazoPropuesta;
                v.estadoContratoInterno = c.ContratoMarco.Estado;
                v.idContratoInterno = c.id;
                v.fechaFinContratoInterno = c.FechaFinContrato;
                v.Centro_de_costos = c.Centro_de_costos;
                v.Sector = c.Sector;
                v.tipoContrato = c.ContratoMarco.TipoDeContrato;
                v.Cuenta = c.ContratoMarco.Cuenta;
                //TODO sacar hardcodeo cuando esten los grupos. Se deja asi porque una empresa "Standalone" se tiene a sí misma como grupo
                v.Grupo = c.ContratoMarco.Cuenta;
                v.plazoContrato = c.ContratoMarco.PlazoPropuesta;
                vehiculosRespuesta.Add(v);
            }
        }
    }
}
