using api.Configuration;
using api.Models.DTO.User;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Route("getPuestosOptions")]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN")]
    public IActionResult GetPuestos()
        => Ok(CargoOptions.OpcionesValidas);

    [HttpGet]
    [Route("GetListaUsuarios")]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN,CONDUCTOR")]
    public async Task<IActionResult> GetListaUsuarios()
        => Ok(await _userService.GetListaUsuarios(User));

    [HttpGet]
    [Route("GetConductores")]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN")]
    public IActionResult GetConductores()
        => Ok(_userService.GetConductores(User));

    [HttpGet("{id}")]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN")]
    public IActionResult GetById([FromRoute] int id)
    {
        var user = _userService.GetUserById(id);

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPost("DesactivarUsuario/{usuarioCrmId}")]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN")]
    public async Task<IActionResult> DesactivarUsuario(string usuarioCrmId)
    {
        await _userService.DesactivarUsuario(usuarioCrmId, User);

        return Ok();
    }

    [HttpPost]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto userDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _userService.CreateUser(userDto, User);

        return Created();
    }
}
