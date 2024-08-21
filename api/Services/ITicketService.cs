using api.Models.DTO.Operaciones;
using api.Models.DTO.Tiquetera;
using api.Models.Entities;

namespace api.Services
{
    public interface ITicketService
    {
        Task<List<TicketDtoResponse>?> GetTickets();
        Task<Ticket> CrearTicket(Ticket ticket);
        Task<List<OrdenTrabajoDto>> GetOrdenesDeTrabajo();
    }
}
