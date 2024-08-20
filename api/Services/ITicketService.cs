using api.Models.DTO.Operaciones;
using api.Models.DTO.Tiquetera;
using api.Models.Entities;

namespace api.Services
{
    public interface ITicketService
    {
        Task<List<Ticket>?> GetTickets();
        Task CrearTicket(TicketDto ticket);
        Task<List<OrdenTrabajoDto>> GetOrdenesDeTrabajo();
    }
}
