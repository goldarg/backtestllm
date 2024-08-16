using api.Models.DTO.Conductor;
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
    private readonly IUserIdentityService _userIdentityService;

    public UsersController(IUserService userService, IUserIdentityService userIdentityService)
    {
        _userService = userService;
        _userIdentityService = userIdentityService;
    }

    [HttpGet("actual")]
    [Authorize]
    public IActionResult GetUsuarioActual()
    {
        var user = _userIdentityService.GetUsuarioDb();
        return Ok(
            new
            {
                Email = user.userName,
                Nombre = user.nombre,
                Apellido = user.apellido,
            }
        );
    }

    [HttpGet]
    [Route("GetListaUsuarios")]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN")]
    public async Task<IActionResult> GetListaUsuarios() =>
        Ok(await _userService.GetListaUsuarios());

    [HttpGet]
    [Route("GetConductores")]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN")]
    public IActionResult GetConductores() => Ok(_userService.GetConductores());

    [HttpGet("{id}")]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN")]
    public IActionResult GetById([FromRoute] int id)
    {
        var user = _userService.GetUserById(id);

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPost("DesactivarUsuario")]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN")]
    public async Task<IActionResult> DesactivarUsuario(DesactivarConductorDto desactivarDto)
    {
        await _userService.DesactivarUsuario(desactivarDto);

        return Ok();
    }

    [HttpPost]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN")]
    public async Task<IActionResult> CreateUser([FromBody] UserDto userDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _userService.CreateUser(userDto);

        return Created();
    }

    [HttpPut]
    [Route("editUser/{usuarioCrmId}")]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN")]
    public async Task<IActionResult> EditUser(string usuarioCrmId, UserDto userDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        await _userService.EditUser(usuarioCrmId, userDto);
        return Ok();
    }

    [HttpPatch]
    [Route("editSelf")]
    [Authorize]
    public async Task<IActionResult> EditSelf([FromBody] UpdateSelfUserDto userDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userName = User?.Identity?.Name;
        if (userName == null)
            return BadRequest("No se pudo obtener el id del usuario actual");

        await _userService.EditSelf(userDto, userName);

        return Ok();
    }
}
