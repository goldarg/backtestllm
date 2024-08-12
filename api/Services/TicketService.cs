using System.Text;
using api.Connected_Services;
using api.DataAccess;
using api.Models.DTO.Empresa;
using api.Models.DTO.Tiquetera;
using api.Models.Entities;
using Newtonsoft.Json;

namespace api.Services
{
    public class TicketService : ITicketService
    {
        private readonly IUserIdentityService _userIdentityService;
        private readonly IRdaUnitOfWork _unitOfWork;
        private readonly TiqueteraService _tiqueteraService;

        public TicketService(
            IUserIdentityService userIdentityService,
            IRdaUnitOfWork unitOfWork,
            TiqueteraService tiqueteraService
        )
        {
            _userIdentityService = userIdentityService;
            _unitOfWork = unitOfWork;
            _tiqueteraService = tiqueteraService;
        }

        public async Task<List<TicketDto>?> GetTickets()
        {
            var uri = new StringBuilder($"api/v1/tickets");

            var responseString = await _tiqueteraService.Get(uri.ToString());

            var ticketsDto = JsonConvert.DeserializeObject<List<TicketDto>>(responseString);

            return ticketsDto;
        }
    }
}
