using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/pruebas")]
[ApiController]
[Authorize]
public class PruebasController : ControllerBase
{
    private readonly IHostEnvironment _env;

    public PruebasController(IHostEnvironment env)
    {
        _env = env;
    }

    [HttpGet("es-desarrollo")]
    public IActionResult EsDesarrollo()
    {
        if (_env.IsDevelopment())
            return Ok("La aplicación está en desarrollo.");

        return Ok("La aplicación NO está en desarrollo.");
    }
}
