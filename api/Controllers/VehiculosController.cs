using System.Text;
using System.Text.Json;
using api.Connected_Services;
using api.DataAccess;
using api.Logic;
using api.Models.DTO;
using api.Models.DTO.Vehiculo;
using api.Models.Entities;
using Microsoft.AspNetCore.Mvc;
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
        else throw new Exception("No se pudo determinar el tipo de contrato del vehículo");

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
        var requestUser = _unitOfWork.GetRepository<User>().GetAll()
            .Where(x => x.id == userId)
            .SingleOrDefault();

        if (requestUser == null)
            throw new Exception("No se encontró el usuario solicitante");

        var empresasDisponibles = requestUser.EmpresasAsignaciones.Select(x => x.Empresa.idCRM).ToList();

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
            ProcessRelatedFields("crm/v2/Alquileres?fields=", ["Dominio_Alquiler", "Conductor", "Contrato", "Estado", "id"], contratos, conductores_Vehiculo),
            // Servicios
            ProcessRelatedFields("crm/v2/Servicios_RDA?fields=", ["Dominio", "Conductor", "Contrato", "Estado", "id"], contratos, conductores_Vehiculo),
            // Renting
            ProcessRelatedFields("crm/v2/Renting?fields=", ["Dominio", "Conductor", "Nombre_del_contrato", "Estado", "id"], contratos, conductores_Vehiculo)
        );

        //Joineo con los 3 modulos para traer el conductor y su respectivo contrato
        vehiculos?.Join(conductores_Vehiculo, v => v.Name, c => c.Dominio.name, (v, c) =>
        {
            v.Conductor = c.Conductor;
            v.Contrato = c.Contrato;
            v.plazoContrato = c.Plazo_Propuesta;
            v.EstadoContrato = c.EstadoContrato;
            v.idContratoInterno = c.contratoIdInterno;
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

    private async Task ProcessRelatedFields(string uri, string[] fields, List<ContratosIdDto> contratos, List<ConductorCuentaVehiculoDto> conductores_Vehiculo)
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

            conductores_Vehiculo.Add(new ConductorCuentaVehiculoDto
            {
                Conductor = conductor,
                Dominio = dominio,
                Contrato = contrato,
                EstadoContrato = estado,
                contratoIdInterno = contratoIdInterno
            });
        }
    }
}