using api.Models.DTO.Tiquetera;
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
    public async Task<IActionResult> CrearTicket(TicketDto ticket)
    {
        await _ticketService.CrearTicket(ticket);
        return StatusCode(201);
    }

    [HttpGet("GetOrdenesDeTrabajo")]
    [Authorize(Roles = "SUPERADMIN,ADMIN,RDA,CONDUCTOR")]
    public async Task<IActionResult> GetOrdenesDeTrabajo()
    {
        return Ok(await _ticketService.GetOrdenesDeTrabajo());
    }

    [HttpGet("GetOTEnCurso")]
    [Authorize(Roles = "SUPERADMIN,ADMIN,RDA,CONDUCTOR")]
    public async Task<IActionResult> GetOTEnCurso()
    {
        return Ok(await _ticketService.GetOTEnCurso());
    }

    [HttpGet("GetOTHistorial")]
    [Authorize(Roles = "SUPERADMIN,ADMIN,RDA,CONDUCTOR")]
    public async Task<IActionResult> GetOTHistorial()
    {
        return Ok(await _ticketService.GetOTHistorial());
    }

    [HttpGet]
    [Authorize(Roles = "SUPERADMIN,ADMIN,RDA,CONDUCTOR")]
    public async Task<IActionResult> GetTickets() => Ok(await _ticketService.GetTickets());

    [HttpGet("GetDetalleOT")]
    [Authorize(Roles = "SUPERADMIN,ADMIN,RDA,CONDUCTOR")]
    public async Task<IActionResult> GetDetalleOT(
        string otCrmId,
        string conductorCrmId,
        string vehiculoCrmId
    )
    {
        return Ok(await _ticketService.GetDetalleOT(otCrmId, conductorCrmId, vehiculoCrmId));
    }
}
