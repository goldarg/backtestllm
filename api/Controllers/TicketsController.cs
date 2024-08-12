using api.Models.DTO.Vehiculo;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpGet]
    [Authorize(Roles = "SUPERADMIN,ADMIN,RDA")]
    public async Task<IActionResult> GetVehiculos() => Ok(await _ticketService.GetTickets());
}
