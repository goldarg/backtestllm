using api.DataAccess;
using api.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/usuariosRoles")]
[ApiController]
public class UsuariosRolesController : ControllerBase
{
    private readonly IRdaUnitOfWork _unitOfWork;

    public UsuariosRolesController(IRdaUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var usuariosPermisos = _unitOfWork.GetRepository<UsuariosRoles>().GetAll()
            .ToList();
        return Ok(usuariosPermisos);
    }

    [HttpGet("{id}")]
    public IActionResult GetById([FromRoute] int id)
    {
        var usuariosPermisos = _unitOfWork.GetRepository<UsuariosRoles>().GetById(id);

        if (usuariosPermisos == null)
            return NotFound();

        return Ok(usuariosPermisos);
    }
}