using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/usuariosEmpresas")]
[ApiController]
[Authorize(Roles = "RDA")]
public class UsuariosEmpresasController : ControllerBase
{
    private readonly IUserService _userService;

    public UsuariosEmpresasController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public IActionResult GetAll()
        => Ok(_userService.GetAllUsuariosEmpresas());   

    [HttpGet]
    [Route("GetEmpresasDelUsuario")]
    public IActionResult GetEmpresasDelUsuario(int usuarioId)
        => Ok(_userService.GetEmpresasDelUsuario(usuarioId));

    [HttpGet("{id}")]
    public IActionResult GetById([FromRoute] int id)
    {
        var usuariosEmpresas = _userService.GetUsuariosEmpresasById(id);

        if (usuariosEmpresas == null)
            return NotFound();

        return Ok(usuariosEmpresas);
    }
}
