using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using api.DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VehiculosController : ControllerBase
{
    // TODO: Definir permisos
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IRdaUnitOfWork _unitOfWork;

    public VehiculosController(IRdaUnitOfWork unitOfWork, IHttpClientFactory httpClientFactory)
    {
        _unitOfWork = unitOfWork;
        _httpClientFactory = httpClientFactory;
    }
    
    [HttpPost]
    public IActionResult AsignarVehiculo()
    {
        var httpClient = _httpClientFactory.CreateClient("CrmHttpClient");
        var uri = new StringBuilder("crm/v2/Contacts");
        //Para el primer MVP solo usamos contactos existentes en el CRM
        // if (user.isRDA)
        // {

        // }

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetVehiculos()
    {
        var httpClient = _httpClientFactory.CreateClient("CrmHttpClient");

        var uri = new StringBuilder("crm/v2/Vehiculos");

        // TODO: Filtrar los contratos segun el rol del usuario.
        // var empresa = string.Empty;

        // uri.Append(false ? $"/search?criteria=(Owner.Name:equals:{empresa})&" : "?");

        // uri.Append("fields=id,Estado,Marca_Vehiculo,Modelo,Versi_n,Chasis,Color,A_o,Pa_s,Medida_Cubierta,Fecha_de_patentamiento,"+
        //     "Od_metro,Poliza_Seguro,Compa_a_de_seguro,Tipo_cobertura,Franquicia,Poliza_N,Fecha_siguiente_VTV,Vencimiento_Matafuego,"+
        //     "Vencimiento_de_Ruta,Padron,Vto_Cedula_verde,Owner");

        var response = await httpClient.GetAsync(uri.ToString());
        var json = await response.Content.ReadAsStringAsync();

        return Ok(json);
    }

    [HttpPost("{id}/AsignarConductor")]
    public async Task<IActionResult> AsignarConductor(int vehiculoId, [FromBody] int userId)
    {
        var httpClient = _httpClientFactory.CreateClient("CrmHttpClient");

        // Deberiamos consultar en que tabla se relacionan los conductores y los vehiculos.
        // Potencialmente parece ser "Vehiculos-drivers".

        // var response = await httpClient.PatchAsync("crm/v2/Vehiculos/" + id, content);

        return Ok();
    }

    // [HttpDelete("EliminarVehiculo")] 
    // public void EliminarVehiculo([FromRoute] int id)
    // {
    //     var httpClient = _httpClientFactory.CreateClient("CrmHttpClient");

    //     var response = httpClient.DeleteAsync("crm/v2/Vehiculos/" + id).Result;
    // }

    // [HttpPost("CrearVehiculo")]
    // public IActionResult CrearVehiculo([FromRoute] CrearVehiculoDto newVehiculo)
    // {
    //     //TODO que campos son requeridos? Segun el CRM solamente el nombre. Esto es asi? Que definieron?
    //     var httpClient = _httpClientFactory.CreateClient("CrmHttpClient");

    //     var options = new JsonSerializerOptions
    //     {
    //         DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    //     };

    //     var json = new StringContent(JsonSerializer.Serialize(newVehiculo, options), Encoding.UTF8, "application/json");

    //     var response = httpClient.PostAsync("crm/v2/Vehiculos", json).Result;

    //     var responseJson = response.Content.ReadAsStringAsync().Result;
    //     return Ok(responseJson);
    // }
}