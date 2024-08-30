using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize(Roles = "RDA,SUPERADMIN,ADMIN,CONDUCTOR")]
public class OpcionesZohoCrmController(IHttpClientFactory httpClientFactory) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> GetOpcionesZohoCrm()
    {
        var cliente = httpClientFactory.CreateClient("SimpleHttpClient");
        var response = await cliente.GetAsync(
            "https://www.zohoapis.com/crm/v2/functions/test2/actions/execute?auth_type=apikey&zapikey=1003.f5e23544d8e7f26befbba5c1900c2d10.e45470758029dd02143979d67a0136ab"
        );

        var content = await response.Content.ReadAsStringAsync();
        return Content(content, "application/json");
    }
}
