using System.Text;
using api.Connected_Services;
using api.DataAccess;
using api.Models.DTO;
using api.Models.DTO.Operaciones;
using api.Models.DTO.Tiquetera;
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

            //Traigo datos del usuario
            var empresasDisponibles = _userIdentityService.ListarEmpresasDelUsuario();
            var userCrmId = _userIdentityService.UserGetCrmId();

            //Busco las OT
            var uri = new StringBuilder(
                "crm/v2/Purchase_Orders?fields=Tracking_Number,PO_Number,"
                    + "Estado_OT_Mirai_fleet,Clasificaci_n,Vehiculo,Cliente,Product_Details,Aprobador,"
                    + "Vendor_Name,Solicitante,Estado_de_presupuesto,Status,Created_Time"
            );

            var json = await _crmService.Get(uri.ToString());
            var ordenesTrabajo = JsonConvert.DeserializeObject<List<OrdenTrabajoDto>>(json);

            //RDA puede ver todo sin filtrado
            if (!_userIdentityService.UsuarioPoseeRol("RDA"))
            {
                //Si es conductor, solamente puede ver los propios
                if (_userIdentityService.UsuarioPoseeRol("CONDUCTOR"))
                {
                    ordenesTrabajo = ordenesTrabajo
                        .Where(x => x.conductor.id == userCrmId)
                        .ToList();
                }
                else //Admin y SuperAdmin filtran segun las empresas que tengan asignadas, aunque fueran todas
                {
                    ordenesTrabajo = ordenesTrabajo
                        .Where(x =>
                            estadosValidos.Contains(x.estadoGeneral)
                            && empresasDisponibles.Contains(x.Cliente?.id)
                        )
                        .OrderByDescending(x => x.fechaCreacion)
                        .ToList();
                }
            }

            return ordenesTrabajo;
        }

        public async Task<List<TicketDtoResponse>?> GetTickets()
        {
            var empresasUsuario = _userIdentityService.ListarEmpresasDelUsuario();

            var tickets = _unitOfWork
                .GetRepository<Ticket>()
                .GetAll()
                .Where(x => empresasUsuario.Contains(x.Empresa.idCRM))
                .Select(x => new TicketDtoResponse
                {
                    numeroTicket = x.numeroTicket,
                    tipoOperacion = x.tipoOperacion,
                    dominio = new CRMRelatedObject { id = x.dominio, name = x.dominio },
                    solicitante = new UserDtoResponse
                    {
                        id = x.Solicitante.idCRM,
                        first_name = x.Solicitante.nombre,
                        last_name = x.Solicitante.apellido
                    }
                })
                .ToList();

            return tickets;
        }
    }
}
