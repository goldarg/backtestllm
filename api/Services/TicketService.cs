using System.Text;
using api.Connected_Services;
using api.DataAccess;
using api.Models.DTO.Operaciones;
using api.Models.Entities;
using Newtonsoft.Json;

namespace api.Services
{
    public class TicketService : ITicketService
    {
        private readonly IUserIdentityService _userIdentityService;
        private readonly IRdaUnitOfWork _unitOfWork;
        private readonly CRMService _crmService;
        private readonly IHttpClientFactory _httpClientFactory;

        public TicketService(
            IUserIdentityService userIdentityService,
            IRdaUnitOfWork unitOfWork,
            CRMService crmService,
            IHttpClientFactory httpClientFactory
        )
        {
            _userIdentityService = userIdentityService;
            _unitOfWork = unitOfWork;
            _crmService = crmService;
            _httpClientFactory = httpClientFactory;
        }

        public Task<Ticket> CrearTicket(Ticket ticket)
        {
            //Creacion en CRM
            throw new NotImplementedException();

            //Creacion Azure
            ticket.idTiquetera = "El id que vaya venido";
            ticket.numeroTicket = "El ticketNumber que vaya venido";
            _unitOfWork.GetRepository<Ticket>().Insert(ticket);
            _unitOfWork.SaveChanges();
        }

        public async Task<List<OrdenTrabajoDto>> GetOrdenesDeTrabajo()
        {
            List<string> estadosValidos = new List<string>
            {
                "Completado",
                "Cancelado",
                "Turno Asignado",
                "En espera de resp cliente",
                "Reprogramar Turno",
                "En espera de resp usuario",
                "Reparando",
                "Abierta"
            };

            //Traigo las OT
            var empresasDisponibles = _userIdentityService.ListarEmpresasDelUsuario();
            if (empresasDisponibles.Count() == 0)
                return null;

            var uri = new StringBuilder(
                "crm/v2/Purchase_Orders?fields=Tracking_Number,PO_Number,"
                    + "Estado_OT_Mirai_fleet,Clasificaci_n,Vehiculo,Cliente,Product_Details,Aprobador,"
                    + "Vendor_Name,Solicitante,Estado_de_presupuesto"
            );

            var json = await _crmService.Get(uri.ToString());
            var ordenesTrabajo = JsonConvert.DeserializeObject<List<OrdenTrabajoDto>>(json);

            // ordenesTrabajo = ordenesTrabajo
            //     .Where(x => estadosValidos.Contains(x.estadoOT))
            //     .ToList();

            return ordenesTrabajo;
        }

        public async Task<List<Ticket>?> GetTickets()
        {
            var empresasUsuario = _userIdentityService.ListarEmpresasDelUsuario();

            var tickets = _unitOfWork
                .GetRepository<Ticket>()
                .GetAll()
                .Where(x => empresasUsuario.Contains(x.Empresa.idCRM))
                .ToList();

            return tickets;
        }
    }
}
