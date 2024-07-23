using api.DataAccess;
using api.Models.Entities;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/roles")]
[ApiController]
public class RolesController : ControllerBase
{
    private readonly IRdaUnitOfWork _unitOfWork;
    private readonly IUserIdentityService _userIdentityService;

    public RolesController(IRdaUnitOfWork unitOfWork, IUserIdentityService userIdentityService)
    {
        _unitOfWork = unitOfWork;
        _userIdentityService = userIdentityService;
    }


    [HttpGet]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN")]
    public IActionResult GetAll()
    {
        var roles = _userIdentityService.ListarRolesInferiores(User);
        var rta = roles.Select(x => new { x.id, x.nombreRol });
        return Ok(rta);
    }

    [HttpGet("{id}")]
    // TODO ESTO NECESITA IMPLEMENTAR JERARQUIA DE ROLES
    [Authorize(Roles = "RDA")]
    public IActionResult GetById([FromRoute] int id)
    {
        var permiso = _unitOfWork.GetRepository<Rol>().GetById(id);

        if (permiso == null)
            return NotFound();

        return Ok(permiso);
    }
}