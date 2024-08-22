using api.Models.DTO.Operaciones;
using api.Models.DTO.Tiquetera;

namespace api.Services
{
    public interface ITicketService
    {
        Task<List<TicketDtoResponse>?> GetTickets();
        Task CrearTicket(TicketDto ticket);
        Task<List<OrdenTrabajoDto>> GetOrdenesDeTrabajo();
    }
}
