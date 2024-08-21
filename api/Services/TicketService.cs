using System.Text;
using api.Connected_Services;
using api.DataAccess;
using api.Models.DTO;
using api.Models.DTO.Operaciones;
using api.Models.DTO.Tiquetera;
using api.Models.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        public async Task CrearTicket(TicketDto ticketDto)
        {
            var idTiquetera = await CrearTicketTiquetera(ticketDto);
            // Aquí puedes hacer lo que necesites con el valor de "id"

            var ticket = new Ticket
            {
                // TODO validar, esto no deberia ser necesario almacenar
                nombreCompleto = "asdf",
                email = ticketDto.email,
                telefono = ticketDto.telefono,
                empresaId = ticketDto.empresaId,
                // TODO validar, no necesitariamos almacenar info extra como el dominioId ?
                dominio = ticketDto.dominio,
                // TODO aca para que almacenamos el departamento, para mostrarlo?, que conviene realmente guardar el id o el nombre?
                departamento = ticketDto.departamentoId,
                tipoOperacion = ticketDto.tipoOperacion,
                asunto = GenerarAsunto(ticketDto),
                zona = ticketDto.zona,
                descripcion = ticketDto.descripcion,
                odometro = ticketDto.odometro,
                turnoOpcion1 = ticketDto.turnoOpcion1,
                turnoOpcion2 = ticketDto.turnoOpcion2,
                idTiquetera = idTiquetera,
                // TODO pendiente ver de donde sale esto
                numeroTicket = "asdfasdf",
                // TODO el id es el de azure o del CRM ?
                solicitanteId = _userIdentityService.GetUsuarioDb().id,
            };
            ////Creacion Azure
            //ticket.idTiquetera = "El id que vaya venido";
            //ticket.numeroTicket = "El ticketNumber que vaya venido";
            //_unitOfWork.GetRepository<Ticket>().Insert(ticket);
            //_unitOfWork.SaveChanges();
        }

        private async Task<string> CrearTicketTiquetera(TicketDto ticketDto)
        {
            var ticketTiquetera = new
            {
                email = ticketDto.email,
                phone = ticketDto.telefono,
                // asunto Dominio + Empresa + Tipo de operación
                subject = GenerarAsunto(ticketDto),
                departmentId = ticketDto.departamentoId, //"474115000172756029", // (Desarrollo de Red)
                // tipo operacion
                classification = ticketDto.tipoOperacion,
                contact = new { email = ticketDto.email },
                accountId = ticketDto.empresaId,
                cf_dominio = ticketDto.dominio,
                cf_zona = ticketDto.zona,
                cf_odometro = ticketDto.odometro,
                cf_turno_alternativa_1 = ticketDto.turnoOpcion1,
                cf_turno_alternativa_2 = ticketDto.turnoOpcion2,
                description = ticketDto.descripcion
            };
            var httpClientTiquetera = _httpClientFactory.CreateClient("TiqueteraHttpClient");

            string jsonTicket = JsonConvert.SerializeObject(ticketTiquetera);
            var content = new StringContent(
                jsonTicket,
                System.Text.Encoding.UTF8,
                "application/json"
            );

            var response = await httpClientTiquetera.PostAsync("tickets", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            var jsonResponse = JObject.Parse(responseContent);

            if (!jsonResponse.ContainsKey("id"))
                throw new Exception("No se pudo crear el ticket en Tiquetera.");

            var idValue = jsonResponse["id"]?.ToString();
            return idValue
                ?? throw new Exception(
                    "La propiedad 'id' no se encontró o es nula en la respuesta."
                );
        }

        private string GenerarAsunto(TicketDto ticketDto)
        {
            return ticketDto.dominio
                + "-"
                + ticketDto.empresaNombre
                + "-"
                + ticketDto.tipoOperacion;
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
                    + "Vendor_Name,Solicitante,Estado_de_presupuesto"
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
                            estadosValidos.Contains(x.estadoOT)
                            && empresasDisponibles.Contains(x.Cliente?.id)
                        )
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
