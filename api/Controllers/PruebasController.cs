using api.DataAccess;
using api.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace api.Controllers;

[Route("api/pruebas")]
[ApiController]
[Authorize]
public class PruebasController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IRdaUnitOfWork _unitOfWork;

    public PruebasController(IRdaUnitOfWork unitOfWork, IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _unitOfWork = unitOfWork;
    }

    [HttpGet("{id}")]
    public IActionResult PruebaUnitOfWork([FromRoute] int id)
    {
        var test = _unitOfWork.GetRepository<User>().GetAll().ToList();

        return Ok(test);
    }

    public class Vehiculo
    {
        public string Codigo { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public bool Asignado { get; init; }
    }

    [HttpGet("GetVehiculos")]
    public IActionResult GetVehiculos(string filter = "all")
    {
        var vehiculos = new List<Vehiculo>
        {
            new() { Codigo = "ABC123", Marca = "Toyota", Asignado = true },
            new() { Codigo = "XYZ789", Marca = "Ford", Asignado = false },
            new() { Codigo = "LMN456", Marca = "Honda", Asignado = true },
            new() { Codigo = "JKL321", Marca = "Nissan", Asignado = false },
            new() { Codigo = "QWE987", Marca = "BMW", Asignado = true },
            new() { Codigo = "RTY654", Marca = "Audi", Asignado = false },
            new() { Codigo = "UIO852", Marca = "Mercedes", Asignado = true },
            new() { Codigo = "GHJ369", Marca = "Chevrolet", Asignado = false },
            new() { Codigo = "ASD741", Marca = "Kia", Asignado = true },
            new() { Codigo = "ZXC159", Marca = "Mazda", Asignado = false },
            new() { Codigo = "VBN753", Marca = "Subaru", Asignado = true },
            new() { Codigo = "POI456", Marca = "Volkswagen", Asignado = false },
            new() { Codigo = "MNB951", Marca = "Hyundai", Asignado = true }
        };

        if (filter == "assigned")
            vehiculos = vehiculos.Where(v => v.Asignado).ToList();
        else if (filter == "unassigned") vehiculos = vehiculos.Where(v => !v.Asignado).ToList();

        return Ok(vehiculos);
    }

    [HttpGet("MetodoPrueba")]
    public IActionResult MetodoPrueba()
    {
        var jsonArray = new JArray
        {
            new JObject
            {
                { "name", "John Doe" },
                { "age", 30 },
                { "email", "john.doe@example.com" },
                { "isVerified", true },
                { "balance", 1234.56 }
            },
            new JObject
            {
                { "name", "Jane Smith" },
                { "age", 25 },
                { "email", "jane.smith@example.com" },
                { "isVerified", false },
                { "balance", 567.89 }
            },
            new JObject
            {
                { "name", "Alice Johnson" },
                { "age", 28 },
                { "email", "alice.johnson@example.com" },
                { "isVerified", true },
                { "balance", 2345.67 }
            }
        };

        var jsonString = JsonConvert.SerializeObject(jsonArray, Formatting.Indented);

        // Print the JSON string
        return Ok(jsonString);
    }

    [HttpGet]
    public IActionResult GetContactsCrm()
    {
        // User.Identity.Name;
        var httpClient = _httpClientFactory.CreateClient("CrmHttpClient");

        var response = httpClient.GetAsync("crm/v2/Contacts").Result;
        var json = response.Content.ReadAsStringAsync().Result;

        return Ok(json);
    }
}