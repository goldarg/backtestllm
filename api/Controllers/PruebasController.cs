using api.data;
using api.Data;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace api.Controllers
{
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

            string jsonString = JsonConvert.SerializeObject(jsonArray, Formatting.Indented);

            // Print the JSON string
            return Ok(jsonString);
        }

        [HttpGet]
        public IActionResult GetContactsCRM()
        {
            var httpClient = _httpClientFactory.CreateClient("CrmHttpClient");

            var response = httpClient.GetAsync("crm/v2/Contacts").Result;
            var json = response.Content.ReadAsStringAsync().Result;

            return Ok(json);
        }
    }
}