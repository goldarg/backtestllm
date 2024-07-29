using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/empresas")]
[ApiController]
public class EmpresasController : ControllerBase
{
    private readonly IEmpresaService _empresaService;
    
    public EmpresasController(IEmpresaService empresaService)
    {
        _empresaService = empresaService;
    }

    [HttpGet]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN")]
    public IActionResult GetAll()
        => Ok(_empresaService.GetAll());


    [HttpGet("{id}")]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN")]
    public IActionResult GetById([FromRoute] int id)
    {
        var empresa = _empresaService.GetById(id);

        if (empresa == null)
            return NotFound();

        return Ok(empresa);
    }
}
