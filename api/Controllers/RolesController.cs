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

    [HttpGet]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN")]
    public IActionResult GetAll()
        => Ok(_rolService.GetAll(User));

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
