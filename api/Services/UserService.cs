using System.Text;
using System.Text.Json;
using api.Configuration;
using api.Connected_Services;
using api.DataAccess;
using api.Exceptions;
using api.Models.DTO;
using api.Models.DTO.Conductor;
using api.Models.DTO.Empresa;
using api.Models.DTO.Rol;
using api.Models.DTO.User;
using api.Models.Entities;
using Newtonsoft.Json.Linq;

namespace api.Services
{
    public class UserService : IUserService
    {
        private readonly IRdaUnitOfWork _unitOfWork;
        private readonly IUserIdentityService _userIdentityService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly CRMService _crmService;

        public UserService(IRdaUnitOfWork unitOfWork, IUserIdentityService userIdentityService, IHttpClientFactory httpClientFactory,
            CRMService crmService)
        {
            _unitOfWork = unitOfWork;
            _userIdentityService = userIdentityService;
            _httpClientFactory = httpClientFactory;
            _crmService = crmService;
        }

        public async Task CreateUser(CreateUserDto userDto, System.Security.Claims.ClaimsPrincipal User)
        {
            var (rolSelected, empresasSelected) = ValidarUsuario(userDto, User);
            var createdId = await CrearUsuarioCRM(userDto);

            _unitOfWork
                .GetRepository<User>()
                .Insert(
                    new User
                    {
                        userName = userDto.Email,
                        nombre = userDto.Nombre,
                        apellido = userDto.Apellido,
                        estado = EstadosUsuario.activo,
                        isRDA = true,
                        idCRM = createdId,
                        Roles =
                        [
                            new() { rolId = rolSelected.id }
                        ],
                        EmpresasAsignaciones = empresasSelected
                            .Select(empresa => new UsuariosEmpresas { empresaId = empresa.id })
                            .ToList()
                    }
                );
            _unitOfWork.SaveChanges();
        }

        public async Task DesactivarUsuario(string usuarioCrmId, System.Security.Claims.ClaimsPrincipal User)
        {
            var maxJerarquiaRequest = _userIdentityService.GetJerarquiaRolMayor(User);

            //Valido que exista en la DB, luego cambio en CRM, luego inactivo en la DB
            var user = _unitOfWork
            .GetRepository<User>()
            .GetAll()
            .Where(x => x.idCRM == usuarioCrmId)
            .SingleOrDefault();

            if (user == null)
                throw new BadRequestException("No se encontró el usuario a eliminar");

            var targetMaxJerarquia =
                user.Roles.Count() > 0 ? user.Roles?.Max(x => x.Rol.jerarquia) : -1;
            if (targetMaxJerarquia >= maxJerarquiaRequest)
                throw new BadRequestException("No se poseen permisos para modificar este usuario");

            var httpClient = _httpClientFactory.CreateClient("CrmHttpClient");
            var jsonObj = new
            {
                data = new[] { new { id = usuarioCrmId, Estado_Mirai = EstadosUsuario.inactivo } }
            };
            var jsonData = JsonSerializer.Serialize(jsonObj);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"crm/v2/Contacts/upsert", content);
            var responseString = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseString);

            if (apiResponse == null || apiResponse.data == null || apiResponse.data.Count == 0)
                throw new BadRequestException("Respuesta inválida del CRM");

            var responseData = apiResponse.data[0];
            if (responseData.status != "success")
            {
                switch (responseData.code)
                {
                    case "DUPLICATE_DATA":
                        throw new BadRequestException(
                            "No se encontró, o no existe, el usuario a editar en el CRM"
                        );
                    default:
                        // TODO habria que loggear responseData.message
                        throw new BadRequestException("Error al editar el usuario en CRM");
                }
            }

            user.estado = EstadosUsuario.inactivo;
            _unitOfWork.GetRepository<User>().Update(user);
            _unitOfWork.SaveChanges();
        }

        public List<ConductorEmpresaDto> GetConductores(System.Security.Claims.ClaimsPrincipal User)
        {
            var empresasDisponibles = _userIdentityService.ListarEmpresasDelUsuario(User);

            return [.. _unitOfWork
                .GetRepository<User>()
                .GetAll()
                .Where(u =>
                    u.Roles.Any(reg => reg.Rol.nombreRol == "CONDUCTOR")
                    && u.EmpresasAsignaciones.Any(ea => empresasDisponibles.Contains(ea.Empresa.idCRM))
                )
                .Select(x => new ConductorEmpresaDto
                {
                    id = x.idCRM,
                    name = x.nombre + " " + x.apellido, //Mismo formato que otroga el CRM
                    empresaId = x.EmpresasAsignaciones.First().Empresa.idCRM
                })];
        }

        public async Task<List<ConductorDto>>? GetListaUsuarios(System.Security.Claims.ClaimsPrincipal User)
        {
            var empresasDisponibles = _userIdentityService.ListarEmpresasDelUsuario(User);
            var topeJerarquia = _userIdentityService.ListarRolesSuperiores(User).Max(x => x.jerarquia);

            //Obtengo los datos necesarios
            var uri = new StringBuilder("crm/v2/Contacts?fields=id,Full_Name,Cargo");

            var json = await _crmService.Get(uri.ToString());
            var conductoresCrm = JsonSerializer.Deserialize<List<ConductorDto>>(json);

            //Get conductores de la DB y joineo con sus roles y empresas
            var conductoresDb = _unitOfWork
                .GetRepository<User>()
                .GetAll()
                .Where(x =>
                    conductoresCrm.Select(y => y.id).ToList().Contains(x.idCRM)
                    && x.Roles.All(x => x.Rol.jerarquia < topeJerarquia) //Si tiene 1 rol superior o igual, no va
                    && x.EmpresasAsignaciones.Any(x => empresasDisponibles.Contains(x.Empresa.idCRM))
                )
                .Select(x => new
                {
                    idCRM = x.idCRM,
                    Roles = x
                        .Roles.Select(x => new RolDto { Id = x.Rol.id, NombreRol = x.Rol.nombreRol })
                        .ToList(),
                    Empresas = x
                        .EmpresasAsignaciones.Select(x => new EmpresaDto
                        {
                            IdCRM = x.Empresa.idCRM,
                            RazonSocial = x.Empresa.razonSocial
                        })
                        .ToList(),
                    estadoDescripcion = x.estado
                })
                .ToList();

            //Con este "inner join" estoy filtrando del CRM los users que no tienen la empresa o roles
            var conductoresPermisosMatch = conductoresCrm
                .Join(
                    conductoresDb,
                    crm => crm.id,
                    db => db.idCRM,
                    (crm, db) =>
                    {
                        crm.Roles = db.Roles;
                        crm.Empresas = db.Empresas;
                        crm.Estado = db.estadoDescripcion;
                        return crm;
                    }
                )
                .ToList();

            var vehiculosConductores = new List<ConductorVehiculoDto>();
            var conductoresCrmIds = conductoresPermisosMatch.Select(x => x.id).ToList();

            await Task.WhenAll(
                // Alquileres
                GetVehiculosPorUsuario(
                    "crm/v2/Alquileres?fields=",
                    ["Conductor", "Dominio_Alquiler"],
                    conductoresCrmIds,
                    vehiculosConductores
                ),
                // Servicios
                GetVehiculosPorUsuario(
                    "crm/v2/Servicios_RDA?fields=",
                    ["Conductor", "Dominio"],
                    conductoresCrmIds,
                    vehiculosConductores
                ),
                // Renting
                GetVehiculosPorUsuario(
                    "crm/v2/Renting?fields=",
                    ["Conductor", "Dominio"],
                    conductoresCrmIds,
                    vehiculosConductores
                )
            );

            return conductoresPermisosMatch
                .GroupJoin(
                    vehiculosConductores,
                    r => r.id,
                    v => v.conductorCrmId,
                    (r, vs) => new { ConductorPermiso = r, Vehiculos = vs }
                )
                .SelectMany(
                    x => x.Vehiculos.DefaultIfEmpty(),
                    (x, v) =>
                    {
                        if (v != null)
                        {
                            x.ConductorPermiso.VehiculosAsignados.Add(v.vehiculo);
                        }
                        return x.ConductorPermiso;
                    }
                )
                .Distinct()
                .ToList();
        }

        public User? GetUserById(int id)
        {
            return _unitOfWork
                .GetRepository<User>()
                .GetAll()
                .Where(x => x.idCRM == id.ToString())
                .SingleOrDefault();
        }

        // Usuarios Empresas
        public List<UsuariosEmpresas>? GetAllUsuariosEmpresas()
            => [.. _unitOfWork.GetRepository<UsuariosEmpresas>().GetAll()];
        
        public List<Empresa?> GetEmpresasDelUsuario(int usuarioId)
            => [.. _unitOfWork
                .GetRepository<UsuariosEmpresas>()
                .GetAll()
                .Where(x => x.userId == usuarioId)
                .Select(x => x.Empresa)];
            
        public UsuariosEmpresas? GetUsuariosEmpresasById(int id)
            => _unitOfWork.GetRepository<UsuariosEmpresas>().GetById(id);

        // Usuarios Roles
        public List<UsuariosRoles>? GetAllUsuariosRoles()
            => [.. _unitOfWork.GetRepository<UsuariosRoles>().GetAll()];

        private async Task GetVehiculosPorUsuario(
                string uri,
                string[] fields,
                List<string> conductoresIds,
                List<ConductorVehiculoDto> vehiculosConductores
            )
        {
            var dataUri = new StringBuilder(uri);
            foreach (var field in fields)
            {
                dataUri.Append(field).Append(",");
            }

            var jsonData = await _crmService.Get(dataUri.ToString().TrimEnd(','));
            var dataArray = JArray.Parse(jsonData);

            foreach (var item in dataArray)
            {
                if (!item[fields[0]].HasValues || !item[fields[1]].HasValues)
                    continue;

                var conductor = item[fields[0]].ToObject<CRMRelatedObject>();
                if (!conductoresIds.Any(c => c == conductor.id))
                    continue;

                var dominio = item[fields[1]].ToObject<CRMRelatedObject>();

                vehiculosConductores.Add(
                    new ConductorVehiculoDto { conductorCrmId = conductor.id, vehiculo = dominio }
                );
            }
        }
        
        /// <summary>
        /// Valida que el usuario a crear sea válido
        /// </summary>
        /// <param name="userDto"></param>
        /// <exception cref="BadRequestException"></exception>
        private (Rol rol, List<Empresa> empresas) ValidarUsuario(CreateUserDto userDto, System.Security.Claims.ClaimsPrincipal User)
        {
            bool isUserExists = _unitOfWork
                .GetRepository<User>()
                .GetAll()
                .Any(x => x.userName == userDto.Email);
            if (isUserExists)
                throw new BadRequestException("El correo electrónico ya está en uso");
            // el rolId tiene que ser un rol válido
            var rolSelected = _unitOfWork
                .GetRepository<Rol>()
                .GetAll()
                .SingleOrDefault(x => x.id == userDto.RolId)
                    ?? throw new BadRequestException("El rol seleccionado no es válido");
            // las empresasIdsCrm tienen que ser empresas válidas
            var empresasSelected = _unitOfWork
                .GetRepository<Empresa>()
                .GetAll()
                .Where(x => x.idCRM != null && userDto.EmpresasIdsCrm.Contains(x.idCRM))
                .ToList();
            if (empresasSelected.Count != userDto.EmpresasIdsCrm.Count)
                throw new BadRequestException(
                    "Al menos una de las empresas seleccionadas no es válida"
                );
            // el usuario actual que crea, debe tener control sobre ese rol
            var rolesInferiores = _userIdentityService.ListarRolesInferiores(User);
            if (!rolesInferiores.Any(x => x.id == rolSelected.id))
                throw new BadRequestException("No tienes permisos para asignar ese rol");
            // si sos conductor solo podes tener una empresa asignada
            if (rolSelected.nombreRol == "CONDUCTOR" && empresasSelected.Count > 1)
                throw new BadRequestException("Un conductor solo puede tener una empresa asignada");
            // el usuario actual que crea, debe tener control sobre esas empresas
            var empresasDisponiblesSegunPermiso = _userIdentityService.ListarEmpresasDelUsuario(User);
            if (!empresasSelected.All(x => empresasDisponiblesSegunPermiso.Contains(x.idCRM)))
                throw new BadRequestException("No tienes permisos para asignar esas empresas");
            // TODO sino sos conductor no podes tener un auto asignado
            return (rolSelected, empresasSelected);
        }

        /// <summary>
        /// Crea un usuario en el CRM
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        /// <exception cref="BadRequestException"></exception>
        private async Task<string> CrearUsuarioCRM(CreateUserDto userDto)
        {
            var httpClient = _httpClientFactory.CreateClient("CrmHttpClient");

            var jsonObj = new
            {
                data = new[]
                {
                new
                {
                    // la cuenta a la que pertenece, no se envia al CRM ya que no puede manejar multiples
                    First_Name = userDto.Nombre,
                    Last_Name = userDto.Apellido,
                    Email = userDto.Email,
                    // puesto
                    Cargo = userDto.Puesto,
                    Phone = userDto.Telefono,
                    Comentario = "cargado desde plataforma"
                }
            }
            };
            var jsonData = JsonSerializer.Serialize(jsonObj);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("crm/v2/Contacts", content);
            var responseString = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseString);

            if (apiResponse == null || apiResponse.data == null || apiResponse.data.Count == 0)
                throw new BadRequestException("Respuesta inválida del CRM");

            var responseData = apiResponse.data[0];
            if (responseData.status != "success")
            {
                switch (responseData.code)
                {
                    case "DUPLICATE_DATA":
                        throw new BadRequestException(
                            "Ya existe un usuario en el CRM con ese correo electrónico"
                        );
                    default:
                        // TODO habria que loggear responseData.message
                        throw new BadRequestException("Error al crear el usuario en CRM");
                }
            }
            var createdId = (responseData?.details?.id)
                ?? throw new BadRequestException("Error al crear el usuario en CRM, no se obtuvo el id");
            return createdId;
        }
    }
}