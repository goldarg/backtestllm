using api.Configuration;
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

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Route("getPuestosOptions")]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN")]
    public IActionResult GetPuestos() => Ok(CargoOptions.OpcionesValidas);

    [HttpGet]
    [Route("GetListaUsuarios")]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN,CONDUCTOR")]
    public async Task<IActionResult> GetListaUsuarios() =>
        Ok(await _userService.GetListaUsuarios(User));

    [HttpGet]
    [Route("GetConductores")]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN")]
    public IActionResult GetConductores() => Ok(_userService.GetConductores(User));

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
        await _userService.DesactivarUsuario(desactivarDto, User);

        return Ok();
    }

    [HttpPost]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN")]
    public async Task<IActionResult> CreateUser([FromBody] UserDto userDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _userService.CreateUser(userDto, User);

        return Created();
    }

    [HttpPut]
    [Route("editUser/{usuarioCrmId}")]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN")]
    public async Task<IActionResult> EditUser(string usuarioCrmId, UserDto userDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        await _userService.EditUser(User, usuarioCrmId, userDto);
        return Ok();
    }

    [HttpPost]
    [Route("editSelfConductor")]
    [Authorize(Roles = "CONDUCTOR")]
    public async Task<IActionResult> EditSelfConductor(
        [FromBody] UpdateSelfConductorDto conductorDto
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userName = User?.Identity?.Name;
        if (userName == null)
            return BadRequest("No se pudo obtener el id del usuario actual");

        await _userService.EditSelfConductor(conductorDto, userName);

        return Ok();
    }
}
