using api.Models.Entities;
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

    [HttpPost]
    [Authorize(Roles = "SUPERADMIN,ADMIN,RDA")]
    public async Task<IActionResult> CrearTicket(Ticket ticket)
    {
        return Ok(await _ticketService.CrearTicket(ticket));
    }

    [HttpGet("GetOrdenesDeTrabajo")]
    [Authorize(Roles = "SUPERADMIN,ADMIN,RDA,CONDUCTOR")]
    public async Task<IActionResult> GetOrdenesDeTrabajo()
    {
        return Ok(await _ticketService.GetOrdenesDeTrabajo());
    }

    [HttpGet]
    [Authorize(Roles = "SUPERADMIN,ADMIN,RDA")]
    public async Task<IActionResult> GetTickets() => Ok(await _ticketService.GetTickets());
}
