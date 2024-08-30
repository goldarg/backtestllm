using System.Text;
using api.Connected_Services;
using api.DataAccess;
using api.Exceptions;
using api.Models.DTO;
using api.Models.DTO.Operaciones;
using api.Models.DTO.Tiquetera;
using api.Models.DTO.User;
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

        public async Task<OrdenTrabajoDetalleDto> GetDetalleOT(
            string otCrmId,
            string conductorCrmId,
            string vehiculoCrmId
        )
        {
            //Traigo datos del usuario
            var empresasDisponibles = _userIdentityService.ListarEmpresasDelUsuario();
            var userCrmId = _userIdentityService.UserGetCrmId();

            //Datos de la OT
            var uri = new StringBuilder(
                $"crm/v2/Purchase_Orders/{otCrmId}?fields=id,PO_Number,Estado_OT_Mirai_fleet,Turno,Vendor_Name,"
                    + "Clasificaciones,Tracking_Number,Fecha_de_Aprobaci_n,Estado_de_presupuesto,Vehiculo,"
                    + "Od_metro,Aprobador,Solicitante,Cliente,Departamento,Created_Time,Grand_Total,"
                    + "Product_Details,Domicilio_Completo,Motivo_de_rechazo_OT"
            );
            var json = await _crmService.Get(uri.ToString());
            var ordenTrabajoDto = JsonConvert
                .DeserializeObject<List<OrdenTrabajoDetalleDto>>(json)
                .First();

            //Datos del vehiculo
            uri = new StringBuilder(
                $"crm/v2/Vehiculos/{vehiculoCrmId}?fields=id,"
                    + "Versi_n,Marca_Vehiculo,Modelo,Pa_s,Name"
            );
            json = await _crmService.Get(uri.ToString());
            var datosVehiculoDto = JsonConvert
                .DeserializeObject<List<DatosDominioDto>>(json)
                .First();
            ordenTrabajoDto.Dominio = datosVehiculoDto;

            //Datos del conductor
            uri = new StringBuilder(
                $"crm/v2/Contacts/{conductorCrmId}?fields=id,Full_Name,Email,Phone"
            );
            json = await _crmService.Get(uri.ToString());
            var datosConductorDto = JsonConvert
                .DeserializeObject<List<DatosConductorDto>>(json)
                .First();
            ordenTrabajoDto.Conductor = datosConductorDto;

            return ordenTrabajoDto;
        }

        public async Task CrearTicket(TicketDto ticketDto)
        {
            // revisar si el usuario tiene asignada la empresa
            var userDb = _userIdentityService.GetUsuarioDb();
            var empresa = userDb
                .EmpresasAsignaciones.Where(e =>
                    e.Empresa != null && e.Empresa.idCRM == ticketDto.empresaCrmId
                )
                .Select(e => e.Empresa)
                .FirstOrDefault();
            if (empresa == null)
                throw new BadRequestException(
                    $"El usuario no tiene asignada la empresa con el idCRM {ticketDto.empresaCrmId}."
                );

            var (idTiquetera, ticketNumber) = await CrearTicketTiquetera(ticketDto);

            var ticket = new Ticket
            {
                empresaId = empresa.id,
                dominioCrmId = ticketDto.dominioCrmId,
                dominio = ticketDto.dominio,
                departamentoCrmId = ticketDto.departamentoCrmId,
                tipoOperacion = ticketDto.tipoOperacion,
                asunto = GenerarAsunto(ticketDto),
                zona = ticketDto.zona,
                descripcion = ticketDto.descripcion,
                odometro = ticketDto.odometro,
                turnoOpcion1 = ticketDto.turnoOpcion1,
                turnoOpcion2 = ticketDto.turnoOpcion2,
                idTiquetera = idTiquetera,
                numeroTicket = ticketNumber,
                solicitanteId = userDb.id,
            };

            _unitOfWork.GetRepository<Ticket>().Insert(ticket);
            _unitOfWork.SaveChanges();
        }

        private async Task<(string id, string ticketNumber)> CrearTicketTiquetera(
            TicketDto ticketDto
        )
        {
            var ticketTiquetera = new
            {
                channel = "Mirai Fleet",
                email = ticketDto.email,
                phone = ticketDto.telefono,
                subject = GenerarAsunto(ticketDto),
                departmentId = ticketDto.departamentoCrmId,
                classification = ticketDto.tipoOperacion,
                contact = new { email = ticketDto.email },
                accountId = ticketDto.empresaCrmId,
                description = ticketDto.descripcion,
                cf = new
                {
                    cf_dominio = ticketDto.dominio,
                    cf_zona = ticketDto.zona,
                    cf_odometro = ticketDto.odometro,
                    cf_turno_alternativa_1 = ticketDto.turnoOpcion1,
                    cf_turno_alternativa_2 = ticketDto.turnoOpcion2,
                }
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
            var ticketNumber = jsonResponse["ticketNumber"]?.ToString();

            if (idValue == null)
                throw new Exception("La propiedad 'id' no se encontró o es nula en la respuesta.");
            if (ticketNumber == null)
                throw new Exception(
                    "La propiedad 'ticketNumber' no se encontró o es nula en la respuesta."
                );

            return (idValue, ticketNumber);
        }

        private string GenerarAsunto(TicketDto ticketDto)
        {
            return "TEST NICOLAS - ENTA - NO TOCAR";
            //return ticketDto.dominio
            //    + "-"
            //    + ticketDto.empresaNombre
            //    + "-"
            //    + ticketDto.tipoOperacion;
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
                    + "Vendor_Name,Solicitante,Estado_de_presupuesto,Status,Created_Time,Conductor_VH"
            );

            var json = await _crmService.Get(uri.ToString());
            var ordenesTrabajo = JsonConvert.DeserializeObject<List<OrdenTrabajoDto>>(json);

            //La foto del conductor al crear la OT se guarda solo como string (name)
            //Entonces con ese dato busco los datos de los que me interesan y luego los mappeo
            uri = new StringBuilder("/crm/v2/contacts?fields=Full_Name,id");
            json = await _crmService.Get(uri.ToString());
            var conductoresCrm = JsonConvert.DeserializeObject<List<UserFullNameDto>>(json);
            var conductoresDict = conductoresCrm.ToDictionary(c => c.Full_Name, c => c);

            var result = ordenesTrabajo //TODO el nombre no es unico, RDA tiene que definir con que campo identificar al conductor
                .Select(orden =>
                {
                    if (conductoresDict.TryGetValue(orden.conductor, out var conductorData))
                    {
                        orden.ConductorData = conductorData;
                    }
                    return orden;
                })
                .ToList();

            //RDA puede ver todo sin filtrado
            if (!_userIdentityService.UsuarioPoseeRol("RDA"))
            {
                //Si es conductor, solamente puede ver los propios
                if (_userIdentityService.UsuarioPoseeRol("CONDUCTOR"))
                {
                    result = result.Where(x => x.Solicitante.id == userCrmId).ToList();
                }
                else //Admin y SuperAdmin filtran segun las empresas que tengan asignadas, aunque fueran todas
                {
                    result = result
                        .Where(x =>
                            estadosValidos.Contains(x.estadoGeneral)
                            && empresasDisponibles.Contains(x.Cliente?.id)
                        )
                        .OrderByDescending(x => x.fechaCreacion)
                        .ToList();
                }
            }

            return result;
        }

        public async Task<List<TicketDtoResponse>?> GetTickets()
        {
            if (_userIdentityService.UsuarioPoseeRol("CONDUCTOR"))
            {
                return _unitOfWork
                    .GetRepository<Ticket>()
                    .GetAll()
                    .Where(x => x.Solicitante.idCRM == _userIdentityService.UserGetCrmId())
                    .Select(x => new TicketDtoResponse
                    {
                        numeroTicket = x.numeroTicket,
                        tipoOperacion = x.tipoOperacion,
                        dominio = new CRMRelatedObject { id = x.dominioCrmId, name = x.dominio },
                        solicitante = new UserDtoResponse
                        {
                            id = x.Solicitante.idCRM,
                            first_name = x.Solicitante.nombre,
                            last_name = x.Solicitante.apellido
                        }
                    })
                    .ToList();
            }

            var empresasUsuario = _userIdentityService.ListarEmpresasDelUsuario();

            var tickets = _unitOfWork
                .GetRepository<Ticket>()
                .GetAll()
                .Where(x => empresasUsuario.Contains(x.Empresa.idCRM))
                .Select(x => new TicketDtoResponse
                {
                    numeroTicket = x.numeroTicket,
                    tipoOperacion = x.tipoOperacion,
                    dominio = new CRMRelatedObject { id = x.dominioCrmId, name = x.dominio },
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

        public async Task<List<OrdenTrabajoDto>> GetOTEnCurso()
        {
            var OTs = await GetOrdenesDeTrabajo();
            var result = new List<OrdenTrabajoDto>();

            foreach (var group in OTs.GroupBy(ot => ot.numeroTicket))
            {
                if (!group.All(ot => ot.estadoOT == "Completado" || ot.estadoOT == "Cancelado"))
                {
                    result.AddRange(group);
                }
            }

            return result;
        }

        public async Task<List<OrdenTrabajoDto>> GetOTHistorial()
        {
            var OTs = await GetOrdenesDeTrabajo();
            var result = new List<OrdenTrabajoDto>();

            foreach (var group in OTs.GroupBy(ot => ot.numeroTicket))
            {
                if (group.All(ot => ot.estadoOT == "Completado" || ot.estadoOT == "Cancelado"))
                {
                    result.AddRange(group);
                }
            }

            return result;
        }
    }
}
