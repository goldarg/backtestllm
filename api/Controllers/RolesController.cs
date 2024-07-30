using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/roles")]
[ApiController]
public class RolesController : ControllerBase
{
    private readonly IRolService _rolService;

    public RolesController(IRolService rolService)
    {
        _rolService = rolService;
    }

    [HttpGet("inferiores")]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN")]
    public IActionResult GetInferiores()
    {
        return Ok(_rolService.GetInferiores());
    }

    [HttpGet("propios")]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN,CONDUCTOR")]
    public IActionResult GetPropios()
    {
        return Ok(_rolService.GetPropios());
    }

    [HttpGet("{id}")]
    // TODO ESTO NECESITA IMPLEMENTAR JERARQUIA DE ROLES
    [Authorize(Roles = "RDA")]
    public IActionResult GetById([FromRoute] int id)
    {
        var permiso = _rolService.GetById(id);

        if (permiso == null)
            return NotFound();

        return Ok(permiso);
    }
}
