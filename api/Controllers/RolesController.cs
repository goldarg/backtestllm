using api.DataAccess;
using api.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/roles")]
[ApiController]
public class RolesController : ControllerBase
{
    private readonly IRdaUnitOfWork _unitOfWork;

    public RolesController(IRdaUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    // TODO ESTO NECESITA IMPLEMENTAR JERARQUIA DE ROLES
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN")]
    public IActionResult GetAll()
    {
        var permisos = _unitOfWork.GetRepository<Rol>().GetAll()
            .ToList();

        return Ok(permisos);
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