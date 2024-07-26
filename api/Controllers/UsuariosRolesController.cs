using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/usuariosRoles")]
[ApiController]
[Authorize(Roles = "RDA")]
public class UsuariosRolesController : ControllerBase
{
    private readonly IUserService _userService;

    public UsuariosRolesController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public IActionResult GetAll()
        => Ok(_userService.GetAllUsuariosRoles());
}
