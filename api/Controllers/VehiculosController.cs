using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using api.DataAccess;
using api.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using YourNamespace;

namespace api.Controllers;

[Route("api/vehiculos")]
[ApiController]
public class VehiculosController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IRdaUnitOfWork _unitOfWork;

    public VehiculosController(IRdaUnitOfWork unitOfWork, IHttpClientFactory httpClientFactory)
    {
        _unitOfWork = unitOfWork;
        _httpClientFactory = httpClientFactory;
    }
    
    [HttpGet("GetVehiculos")]
    public IActionResult GetVehiculos()
    {
        var httpClient = _httpClientFactory.CreateClient("CrmHttpClient");

        var response = httpClient.GetAsync("crm/v2/Vehiculos").Result;
        var json = response.Content.ReadAsStringAsync().Result;

        return Ok(json);
    }

    [HttpPost("ActualizarVehiculo")]
    public IActionResult ActualizarVehiculo([FromRoute] int id, [FromBody]VehiculoDto vehiculo)
    {
        //TDDO pueden modificar cualquier campo? Qué campos si y qué campos no? Son todos los del CRM?
        
        var jsonContent = JsonSerializer.Serialize(vehiculo, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var httpClient = _httpClientFactory.CreateClient("CrmHttpClient");

        var response = httpClient.PatchAsync("crm/v2/Vehiculos/" + id, content).Result;

        return Ok();
    }

    [HttpDelete("EliminarVehiculo")]
    public void EliminarVehiculo([FromRoute] int id)
    {
        var httpClient = _httpClientFactory.CreateClient("CrmHttpClient");

        var response = httpClient.DeleteAsync("crm/v2/Vehiculos/" + id).Result;
    }

    [HttpPost("CrearVehiculo")]
    public IActionResult CrearVehiculo([FromRoute] CrearVehiculoDto newVehiculo)
    {
        //TODO que campos son requeridos? Segun el CRM solamente el nombre. Esto es asi? Que definieron?
        var httpClient = _httpClientFactory.CreateClient("CrmHttpClient");

        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var json = new StringContent(JsonSerializer.Serialize(newVehiculo, options), Encoding.UTF8, "application/json");

        var response = httpClient.PostAsync("crm/v2/Vehiculos", json).Result;

        var responseJson = response.Content.ReadAsStringAsync().Result;
        return Ok(responseJson);
    }
}