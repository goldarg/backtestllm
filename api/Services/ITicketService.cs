using api.Models.DTO.Empresa;
using api.Models.DTO.Tiquetera;
using api.Models.Entities;

namespace api.Services
{
    public interface ITicketService
    {
        Task<List<TicketDto>?> GetTickets();
    }
}
