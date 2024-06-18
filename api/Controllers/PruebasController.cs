using api.DataAccess;
using api.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace api.Controllers;

[Route("api/pruebas")]
[ApiController]
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
        var test = _unitOfWork.GetRepository<Comment>().GetAll()
            .Select(x => x.Stock).ToList();

        return Ok(test);
    }

    [HttpGet("GetVehiculos")]
    public IActionResult GetVehiculos()
    {
        var jsonArray = new JArray
        {
            new JObject
            {
                { "name", "John Doe" },
                { "age", 30 },
                { "email", "john.doe@example.com" },
                { "isVerified", true },
                { "balance", 1234.56 },
                { "asignado", 1 }
            },
            new JObject
            {
                { "name", "Jane Smith" },
                { "age", 25 },
                { "email", "jane.smith@example.com" },
                { "isVerified", false },
                { "balance", 567.89 },
                { "asignado", 0 }
            },
            new JObject
            {
                { "name", "Alice Johnson" },
                { "age", 28 },
                { "email", "alice.johnson@example.com" },
                { "isVerified", true },
                { "balance", 2345.67 },
                { "asignado", 1 }
            },
            new JObject
            {
                { "name", "Robert Brown" },
                { "age", 32 },
                { "email", "robert.brown@example.com" },
                { "isVerified", false },
                { "balance", 789.01 },
                { "asignado", 0 }
            },
            new JObject
            {
                { "name", "Emily Davis" },
                { "age", 27 },
                { "email", "emily.davis@example.com" },
                { "isVerified", true },
                { "balance", 234.56 },
                { "asignado", 1 }
            },
            new JObject
            {
                { "name", "Michael Wilson" },
                { "age", 35 },
                { "email", "michael.wilson@example.com" },
                { "isVerified", false },
                { "balance", 4567.89 },
                { "asignado", 0 }
            },
            new JObject
            {
                { "name", "Sarah Lee" },
                { "age", 24 },
                { "email", "sarah.lee@example.com" },
                { "isVerified", true },
                { "balance", 123.45 },
                { "asignado", 1 }
            },
            new JObject
            {
                { "name", "David Clark" },
                { "age", 29 },
                { "email", "david.clark@example.com" },
                { "isVerified", false },
                { "balance", 678.90 },
                { "asignado", 0 }
            },
            new JObject
            {
                { "name", "Laura Martinez" },
                { "age", 26 },
                { "email", "laura.martinez@example.com" },
                { "isVerified", true },
                { "balance", 345.67 },
                { "asignado", 1 }
            },
            new JObject
            {
                { "name", "Chris Taylor" },
                { "age", 31 },
                { "email", "chris.taylor@example.com" },
                { "isVerified", false },
                { "balance", 890.12 },
                { "asignado", 0 }
            }
        };

        var jsonString = JsonConvert.SerializeObject(jsonArray, Formatting.Indented);

        return Ok(jsonString);
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
        var httpClient = _httpClientFactory.CreateClient("CrmHttpClient");

        var response = httpClient.GetAsync("crm/v2/Contacts").Result;
        var json = response.Content.ReadAsStringAsync().Result;

        return Ok(json);
    }
}