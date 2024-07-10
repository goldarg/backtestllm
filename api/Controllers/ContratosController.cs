using System.Text;
using api.DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/[controller]")]
    public class ContratosController : Controller
    {
        // TODO: Permisos a definir 
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IRdaUnitOfWork _unitOfWork;

        public ContratosController(IHttpClientFactory httpClientFactory, IRdaUnitOfWork unitOfWork)
        {
            _httpClientFactory = httpClientFactory;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetContratos()
        {
            var httpClient = _httpClientFactory.CreateClient("CrmHttpClient");

            var uri = new StringBuilder("crm/v2/Contratos");

            // TODO: Filtrar los contratos segun el rol del usuario.
            // var empresa = string.Empty;

            // uri.Append(false ? $"/search?criteria=(Cuenta.Name:equals:{empresa})&" : "?");

            // uri.Append("fields=Plazo_en_d_as,Vehiculos_Activos_Renting,Z_Fecha_Finalizacion,Cantidad_de_unidades,Tipo_de_Contrato,Nro_Contrato," +
            //     "Vehiculos_Activos,Estado,Fecha_Inicio_Propuesta,Plazo_Propuesta,Facturacion_Mensual,Currency");

            var response = await httpClient.GetAsync(uri.ToString());
            var json = await response.Content.ReadAsStringAsync();

            return Ok(json);
        }
    }
}