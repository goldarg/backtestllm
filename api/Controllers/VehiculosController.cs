using api.Models.DTO.Vehiculo;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VehiculosController : ControllerBase
{
    private readonly IVehiculoService _vehiculoService;

    public VehiculosController(IVehiculoService vehiculoService)
    {
        _vehiculoService = vehiculoService;
    }

    [HttpPost]
    [Route("AsignarVehiculo")]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN")]
    public async Task<IActionResult> AsignarVehiculo([FromBody] AsignarVehiculoDto asignarVehiculoDto)
        => Ok(await _vehiculoService.AsignarVehiculo(asignarVehiculoDto));

    [HttpGet]
    [Authorize(Roles = "SUPERADMIN,ADMIN,RDA")]
    public async Task<IActionResult> GetVehiculos()
        => Ok(await _vehiculoService.GetVehiculos(User));

    [HttpGet("HistorialOperaciones")]
    [Authorize(Roles = "SUPERADMIN,ADMIN,RDA")]
    public async Task<IActionResult> GetHistorialOperaciones([FromQuery] string dominio, [FromQuery] string tipoContrato)
        => Ok(await _vehiculoService.HistorialOperaciones(User, dominio, tipoContrato));
}
