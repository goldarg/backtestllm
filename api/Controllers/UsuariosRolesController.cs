using api.DataAccess;
using api.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/usuariosRoles")]
[ApiController]
[Authorize(Roles = "RDA")]
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
}