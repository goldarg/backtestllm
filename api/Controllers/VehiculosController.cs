using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using api.DataAccess;
using api.Logic;
using api.Models.DTO;
using api.Models.DTO.Vehiculo;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VehiculosController : ControllerBase
{
    // TODO: Definir permisos
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IRdaUnitOfWork _unitOfWork;
    private readonly VehiculosLogica _logica;

    public VehiculosController(IRdaUnitOfWork unitOfWork, IHttpClientFactory httpClientFactory)
    {
        _unitOfWork = unitOfWork;
        _httpClientFactory = httpClientFactory;
        _logica = new VehiculosLogica(_httpClientFactory);
    }
    
    [HttpPost]
    public async Task<IActionResult> AsignarVehiculo([FromBody]int contratoId, [FromBody]int usuarioId, [FromBody]string tipoContrato)
    {
        var httpClient = _httpClientFactory.CreateClient("CrmHttpClient");

        //Busco y actualizo según el tipo de contrato
        string targetModule;
        if (tipoContrato == "Fleet Management")
            tipoContrato = "Servicios_RDA";
        else if (tipoContrato== "Renting")
            tipoContrato = "Renting";
        else if (tipoContrato == "Alquiler Corporativo")
            tipoContrato = "Alquileres";
        else throw new Exception("No se pudo determinar el tipo de contrato del vehículo");

        var uri = new StringBuilder($"crm/v2/{tipoContrato}/upsert");

        //Armo el objeto para enviar al CRM, y devuelvo la respuesta
        var jsonObject = new
        {
            data = new[]
            {
                new
                {
                    id = contratoId,
                    Conductor = new
                    {
                        id = usuarioId
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
    public async Task<IActionResult> GetVehiculos()
    {
        var httpClient = _httpClientFactory.CreateClient("CrmHttpClient");

        var uri = new StringBuilder("crm/v2/Vehiculos");

        // TODO: Filtrar los contratos segun el rol del usuario.

        //Get a Vehiculos con los datos que necesito
        uri.Append("?fields=id,Estado,Marca_Vehiculo,Modelo,Versi_n,Chasis,Color,A_o,Medida_Cubierta,"+
            "Fecha_de_patentamiento,Compa_a_de_seguro,Franquicia,Poliza_N,Vencimiento_Matafuego,"+
            "Vencimiento_de_Ruta,Padron,Vto_Cedula_Verde");

        var response = await httpClient.GetAsync(uri.ToString());
        var json = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        var vehiculos = JsonSerializer.Deserialize<VehiculoResponse>(json, options);

        //Hasta aca tenemos los vehiculos. Ahora empiezo a buscar los conductores de cada uno.
        //Para eso, primero busco los contratos, y agrupo por tipo de contrato:

        //Cuando se defina cómo es el tema de los estados, sería tan sencillo como agregar un
        //and(Estado:equals:Talcosa) al final del request
        uri = new StringBuilder("crm/v2/Contratos/search?criteria=" + 
            "((Tipo_de_Contrato:equals:Renting)or(Tipo_de_Contrato:equals:Fleet Management)or" + 
            "(Tipo_de_Contrato:equals:Alquiler Corporativo))&fields=id,Tipo_de_Contrato");

        response = await httpClient.GetAsync(uri.ToString());
        json = await response.Content.ReadAsStringAsync();
        var contratos = JsonSerializer.Deserialize<ContratosIdResponse>(json, options)
        .Data.GroupBy(x => x.Tipo_de_Contrato).ToList();

        //Alquileres
        //Get a alquileres, donde Contrato.ID esté contenido en los ID de los contratos
        //Select a Dominio_Alquiler.id , y Conductor (el objeto)

        //Servicios
        //Get a Servicios, donde Contrato.ID este contenido en los ID de los contratos
        //select a Dominio.Id, y Conductor (el objeto)

        //Renting
        //Get a Renting, donde Nombre_del_contrato.ID este contenido en los ID de los contratos
        //select a Dominio.ID, y Conductor (el objeto)

        //Joineo esas 3 listas con el GET a vehiculos (por dominioId), y le pongo el respectivo conductor

        //Devolver ese objeto

        return Ok(vehiculos);
    }
}