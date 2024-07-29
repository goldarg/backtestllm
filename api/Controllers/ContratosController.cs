using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/[controller]")]
public class ContratosController : Controller
{
    private readonly IContratoService _contratoService;
    
    public ContratosController(IContratoService contratoService)
    {
        _contratoService = contratoService;
    }

    [HttpGet]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN")]
    public async Task<IActionResult> GetContratos()
        => Ok(await _contratoService.GetContratos());
}
