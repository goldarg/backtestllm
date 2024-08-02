using System.Security.Cryptography;
using api.DataAccess;
using api.Models.Entities;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "RDA,SUPERADMIN,ADMIN,CONDUCTOR")]
public class ActividadUsuariosController : ControllerBase
{
    private readonly IActividadUsuarioService _actividadUsuarioService;

    public ActividadUsuariosController(IActividadUsuarioService actividadUsuarioService)
    {
        _actividadUsuarioService = actividadUsuarioService;
    }

    [HttpGet("ActividadUsuarioEjecutor")]
    public IActionResult GetActividadUsuarioEjecutor()
    {
        return Ok(_actividadUsuarioService.GetActividadUsuarioEjecutor());
    }

    [HttpGet("ActividadUsuarioAfectado")]
    public IActionResult GetActividadUsuarioAfectado()
    {
        return Ok(_actividadUsuarioService.GetActividadUsuarioAfectado());
    }
}
