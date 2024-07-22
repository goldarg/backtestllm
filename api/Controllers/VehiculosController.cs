using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using api.Connected_Services;
using api.DataAccess;
using api.Exceptions;
using api.Logic;
using api.Models.DTO;
using api.Models.DTO.Operaciones;
using api.Models.DTO.Vehiculo;
using api.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json.Linq;

namespace api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VehiculosController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly CRMService _crmService;
    private readonly IRdaUnitOfWork _unitOfWork;
    private readonly VehiculosLogica _logica;

    public VehiculosController(IRdaUnitOfWork unitOfWork, IHttpClientFactory httpClientFactory, CRMService crmService)
    {
        _unitOfWork = unitOfWork;
        _httpClientFactory = httpClientFactory;
        _logica = new VehiculosLogica(_httpClientFactory);
        _crmService = crmService;
    }

    [HttpPost]
    [Route("AsignarVehiculo")]
    public async Task<IActionResult> AsignarVehiculo([FromBody] AsignarVehiculoDto asignarVehiculoDto)
    {
        var httpClient = _httpClientFactory.CreateClient("CrmHttpClient");

        if (asignarVehiculoDto.usuarioId == null)
        {
            asignarVehiculoDto.usuarioId = _unitOfWork.GetRepository<User>().GetAll()
                .Where(x => x.nombre == "Sin" && x.apellido == "Asignar").Single().idCRM;
        }

        //Busco y actualizo según el tipo de contrato
        string targetModule;
        if (asignarVehiculoDto.tipoContrato == "Fleet Management")
            asignarVehiculoDto.tipoContrato = "Servicios_RDA";
        else if (asignarVehiculoDto.tipoContrato == "Renting")
            asignarVehiculoDto.tipoContrato = "Renting";
        else if (asignarVehiculoDto.tipoContrato == "Alquiler Corporativo")
            asignarVehiculoDto.tipoContrato = "Alquileres";
        else throw new BadRequestException("No se pudo determinar el tipo de contrato del vehículo");

        var uri = new StringBuilder($"crm/v2/{asignarVehiculoDto.tipoContrato}/upsert");

        //Armo el objeto para enviar al CRM, y devuelvo la respuesta
        var jsonObject = new
        {
            data = new[]
            {
                new
                {
                    id = asignarVehiculoDto.idContratoInterno,
                    Conductor = new
                    {
                        id = asignarVehiculoDto.usuarioId
                    }
                }
            }
        };

        string jsonString = JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions { WriteIndented = true });
        HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(uri.ToString(), content);
        var json = await response.Content.ReadAsStringAsync();
        return Ok(json);
    }

    [HttpGet]
    public async Task<IActionResult> GetVehiculos()
    {
        // TODO: Obtener id/empresa/username del usuario
        var userId = 3; //Placeholder por ahora

        //Busco el usuario y las empresas que puede ver
        var empresasAsignaciones = _unitOfWork.GetRepository<User>().GetAll()
            .Where(x => x.id == userId)
            .Select(x => x.EmpresasAsignaciones)
            .SingleOrDefault();

        if (empresasAsignaciones == null)
            throw new BadRequestException("No se encontró el usuario solicitante");

        var empresasDisponibles = empresasAsignaciones.Select(x => x.Empresa.idCRM).ToList();

        //Get a Vehiculos con los datos que necesito
        var uri = new StringBuilder("crm/v2/Vehiculos?fields=id,Name,Estado,Marca_Vehiculo,Modelo,Versi_n,Chasis,Color,A_o,Medida_Cubierta," +
            "Fecha_de_patentamiento,Compa_a_de_seguro,Franquicia,Poliza_N,Vencimiento_Matafuego," +
            "Vencimiento_de_Ruta,Padron,Vto_Cedula_Verde,Ultimo_Odometro_KM,Fecha_siguiente_VTV,Pa_s,Tipo_cobertura");

        var json = await _crmService.Get(uri.ToString());
        var vehiculos = JsonSerializer.Deserialize<List<VehiculoDto>>(json);

        //Hasta aca tenemos los vehiculos. Ahora empiezo a buscar los conductores de cada uno.
        //Para eso, primero busco los contratos, y agrupo por tipo de contrato:

        //Cuando se defina cómo es el tema de los estados, sería tan sencillo como agregar un
        //and(Estado:equals:Talcosa) al final del request
        uri = new StringBuilder("crm/v2/Contratos/search?criteria=" +
            "((Tipo_de_Contrato:equals:Renting)or(Tipo_de_Contrato:equals:Fleet Management)or" +
            "(Tipo_de_Contrato:equals:Alquiler Corporativo))&fields=id,Tipo_de_Contrato,Cuenta,Plazo_Propuesta");

        json = await _crmService.Get(uri.ToString());
        var contratos = JsonSerializer.Deserialize<List<ContratosIdDto>>(json);

        var conductores_Vehiculo = new List<ConductorCuentaVehiculoDto>();

        await Task.WhenAll(
            // Alquileres
            ProcessRelatedFields("crm/v2/Alquileres?fields=", ["Dominio_Alquiler", "Conductor", "Contrato", "Estado", "id", "Fecha_de_Devolucion"], contratos, conductores_Vehiculo),
            // Servicios
            ProcessRelatedFields("crm/v2/Servicios_RDA?fields=", ["Dominio", "Conductor", "Contrato", "Estado", "id", "Fin_de_servicio"], contratos, conductores_Vehiculo),
            // Renting
            ProcessRelatedFields("crm/v2/Renting?fields=", ["Dominio", "Conductor", "Nombre_del_contrato", "Estado", "id", "Fecha_fin_de_renting"], contratos, conductores_Vehiculo)
        );

        //Joineo con los 3 modulos para traer el conductor y su respectivo contrato
        vehiculos?.Join(conductores_Vehiculo, v => v.Name, c => c.Dominio.name, (v, c) =>
        {
            v.Conductor = c.Conductor;
            v.Contrato = c.Contrato;
            v.plazoContrato = c.Plazo_Propuesta;
            v.estadoContratoInterno = c.estadoContratoInterno;
            v.idContratoInterno = c.contratoIdInterno;
            v.fechaFinContratoInterno = c.FechaFinContratoInterno;
            return v;
        }).ToList();

        //Joineo con los datos de Contratos para saber el tipo de contrato que representa
        vehiculos?.Join(contratos, v => v.Contrato.id, c => c.id, (v, c) =>
        {
            v.tipoContrato = c.Tipo_de_Contrato;
            v.Cuenta = c.Cuenta;
            v.Grupo = c.Cuenta; //TODO sacar hardcodeo cuando esten los grupos. Se deja asi porque una empresa "Standalone" se tiene a sí misma como grupo
            v.plazoContrato = c.Plazo_Propuesta;
            return v;
        }).ToList();

        vehiculos = vehiculos.Where(x => empresasDisponibles.Contains(x.Cuenta.id)).ToList();

        return Ok(vehiculos);
    }

    [HttpGet("/HistorialOperaciones")]
    public async Task<IActionResult> GetHistorialOperaciones([FromQuery] string dominio, [FromQuery] string tipoContrato)
    {
        // TODO: Obtener id/empresa/username del usuario
        var userId = 3; //Placeholder por ahora

        //Busco el usuario y las empresas que puede ver
        var empresasAsignaciones = _unitOfWork.GetRepository<User>().GetAll()
            .Where(x => x.id == userId)
            .Select(x => x.EmpresasAsignaciones)
            .SingleOrDefault() ?? throw new BadRequestException("No se encontró el usuario solicitante");

        var empresasDisponibles = empresasAsignaciones.Select(x => x.Empresa.idCRM).ToList();

        var uris = new Dictionary<string, string>
        {
            { "Alquiler Corporativo", $"crm/v2/Alquileres/search?criteria=(Dominio_Alquiler.name:equals:" + dominio + ")&fields=Dominio_Alquiler,Contrato" },
            { "Fleet Management", $"crm/v2/Servicios_RDA/search?criteria=(Dominio.name:equals:" + dominio + ")&fields=Dominio,Contrato" },
            { "Renting", $"crm/v2/Renting/search?criteria=(Dominio.name:equals:" + dominio + ")&fields=Dominio,Nombre_del_contrato" }
        };

        var json = await _crmService.Get(uris[tipoContrato]);
        var contratoId = JArray.Parse(json)[0][tipoContrato == "Renting" ? "Nombre_del_contrato" : "Contrato"].ToObject<CRMRelatedObject>().id;

        var uri = new StringBuilder("crm/v2/Contratos/search?criteria=(Id:equals:" + contratoId + ")&fields=Cuenta");

        json = await _crmService.Get(uri.ToString());

        var contrato = JsonSerializer.Deserialize<List<ContratosIdDto>>(json)[0];

        if (!empresasDisponibles.Contains(contrato.Cuenta.id))
            throw new Exception("No tiene permisos para ver este contrato");

        uri = new StringBuilder("crm/v2/Purchase_Orders/search?criteria=(Vehiculo.name:equals:" + dominio + ")" +
            "&fields=id,Clasificaciones,Vehiculo,Product_Details,Vendor_Name,Turno,Status,PO_Number");

        json = await _crmService.Get(uri.ToString());
        var operaciones = JsonSerializer.Deserialize<List<OperacionesResponseDto>>(json);

        var response = operaciones.Select(o => new OperacionesVehiculoDto
        {
            Id = o.Id,
            TipoOperacion = o.TipoOperacion,
            Detalle = o.Detalle.Any() ? o.Detalle.Select(d => d.Product.Name).ToList() : null,
            Taller = o.Taller?.name,
            FechaTurno = o.FechaTurno,
            Estado = o.Estado,
            OT = o.OT
        });

        return Ok(response);
    }

    private async Task ProcessRelatedFields(string uri, string[] fields, List<ContratosIdDto>? contratos, List<ConductorCuentaVehiculoDto>? conductores_Vehiculo)
    {
        var dataUri = new StringBuilder(uri);
        foreach (var field in fields)
        {
            dataUri.Append(field).Append(",");
        }

        var jsonData = await _crmService.Get(dataUri.ToString().TrimEnd(','));
        var dataArray = JArray.Parse(jsonData);

        foreach (var item in dataArray)
        {
            var contrato = item[fields[2]].ToObject<CRMRelatedObject>();
            if (!contratos.Any(c => c.id == contrato.id))
                return;

            var conductor = item[fields[1]].ToObject<CRMRelatedObject>();
            var dominio = item[fields[0]].ToObject<CRMRelatedObject>();
            var estado = item[fields[3]].ToObject<string>();
            var contratoIdInterno = item[fields[4]].ToObject<string>();
            var fechaFinContratoInterno = item[fields[5]].ToObject<DateTime?>();

            conductores_Vehiculo.Add(new ConductorCuentaVehiculoDto
            {
                Conductor = conductor,
                Dominio = dominio,
                Contrato = contrato,
                estadoContratoInterno = estado,
                contratoIdInterno = contratoIdInterno,
                FechaFinContratoInterno = fechaFinContratoInterno
            });
        }
    }
}