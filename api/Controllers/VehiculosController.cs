using System.Text;
using System.Text.Json;
using api.Connected_Services;
using api.DataAccess;
using api.Logic;
using api.Models.DTO;
using api.Models.DTO.Vehiculo;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VehiculosController : ControllerBase
{
    // TODO: Definir permisos
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
    public async Task<IActionResult> AsignarVehiculo([FromBody] AsignarVehiculoRequest asignarVehiculoRequest)
    {
        var httpClient = _httpClientFactory.CreateClient("CrmHttpClient");

        //Busco y actualizo según el tipo de contrato
        string targetModule;
        if (asignarVehiculoRequest.tipoContrato == "Fleet Management")
            asignarVehiculoRequest.tipoContrato = "Servicios_RDA";
        else if (asignarVehiculoRequest.tipoContrato == "Renting")
            asignarVehiculoRequest.tipoContrato = "Renting";
        else if (asignarVehiculoRequest.tipoContrato == "Alquiler Corporativo")
            asignarVehiculoRequest.tipoContrato = "Alquileres";
        else throw new Exception("No se pudo determinar el tipo de contrato del vehículo");

        var uri = new StringBuilder($"crm/v2/{asignarVehiculoRequest.tipoContrato}/upsert");

        //Armo el objeto para enviar al CRM, y devuelvo la respuesta
        var jsonObject = new
        {
            data = new[]
            {
                new
                {
                    id = asignarVehiculoRequest.contratoId,
                    Conductor = new
                    {
                        id = asignarVehiculoRequest.usuarioId
                    }
                }
            }
        };

        string jsonString = JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions { WriteIndented = true });
        var response = await httpClient.GetAsync(uri.ToString());
        var json = await response.Content.ReadAsStringAsync();
        return Ok(json);
    }

    [HttpGet]
    public async Task<IActionResult> PruebaGetVehiculos()
    {
        var httpClient = _httpClientFactory.CreateClient("CrmHttpClient");

        var uri = new StringBuilder("crm/v2/Vehiculos");
        uri.Append("?fields=id,Estado,Marca_Vehiculo,Modelo,Versi_n,Chasis,Color,A_o,Medida_Cubierta,"+
            "Fecha_de_patentamiento,Compa_a_de_seguro,Franquicia,Poliza_N,Vencimiento_Matafuego,"+
            "Vencimiento_de_Ruta,Padron,Vto_Cedula_Verde");

        var json = await _crmService.Get(uri.ToString());
        var vehiculos = JsonSerializer.Deserialize<List<VehiculoDto>>(json);
        
        return Ok(vehiculos);
    }

    [HttpGet]
    [Route("Vehiculos/Prueba")] // TODO: Convertir este EP en el GET genérico.
    public async Task<IActionResult> GetVehiculos()
    {
        // TODO: Filtrar los contratos segun el rol del usuario.
        //Get a Vehiculos con los datos que necesito
        var uri = new StringBuilder("crm/v2/Vehiculos?fields=id,Name,Estado,Marca_Vehiculo,Modelo,Versi_n,Chasis,Color,A_o,Medida_Cubierta," +
            "Fecha_de_patentamiento,Compa_a_de_seguro,Franquicia,Poliza_N,Vencimiento_Matafuego," +
            "Vencimiento_de_Ruta,Padron,Vto_Cedula_Verde");

        var json = await _crmService.Get(uri.ToString());
        var vehiculos = JsonSerializer.Deserialize<List<VehiculoDto>>(json);

        //Hasta aca tenemos los vehiculos. Ahora empiezo a buscar los conductores de cada uno.
        //Para eso, primero busco los contratos, y agrupo por tipo de contrato:

        //Cuando se defina cómo es el tema de los estados, sería tan sencillo como agregar un
        //and(Estado:equals:Talcosa) al final del request
        uri = new StringBuilder("crm/v2/Contratos/search?criteria=" +
            "((Tipo_de_Contrato:equals:Renting)or(Tipo_de_Contrato:equals:Fleet Management)or" +
            "(Tipo_de_Contrato:equals:Alquiler Corporativo))&fields=id,Tipo_de_Contrato");

        json = await _crmService.Get(uri.ToString());
        var contratos = JsonSerializer.Deserialize<List<ContratosIdDto>>(json);

        var conductores_Vehiculo = new List<ConductorCuentaVehiculoDto>();

        await Task.WhenAll(
            // Alquileres
            ProcessRelatedFields("crm/v2/Alquileres?fields=", ["Dominio_Alquiler", "Conductor", "Contrato"], contratos, conductores_Vehiculo),
            // Servicios
            ProcessRelatedFields("crm/v2/Servicios_RDA?fields=", ["Dominio", "Conductor", "Contrato"], contratos, conductores_Vehiculo),
            // Renting
            ProcessRelatedFields("crm/v2/Renting?fields=", ["Dominio", "Conductor", "Nombre_del_contrato"], contratos, conductores_Vehiculo)
        );

        vehiculos?.Join(conductores_Vehiculo, v => v.Name, c => c.Dominio.name, (v, c) =>
        {
            v.Conductor = c.Conductor;
            return v;
        }).ToList();

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

            conductores_Vehiculo.Add(new ConductorCuentaVehiculoDto
            {
                Conductor = conductor,
                Dominio = dominio
            });
        }
    }
}