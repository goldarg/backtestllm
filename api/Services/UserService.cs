using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using api.Configuration;
using api.Connected_Services;
using api.DataAccess;
using api.Exceptions;
using api.Models.DTO;
using api.Models.DTO.Common;
using api.Models.DTO.Conductor;
using api.Models.DTO.Empresa;
using api.Models.DTO.Rol;
using api.Models.DTO.User;
using api.Models.DTO.Vehiculo;
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
        private readonly IActividadUsuarioService _actividadUsuarioService;

        public UserService(
            IRdaUnitOfWork unitOfWork,
            IUserIdentityService userIdentityService,
            IHttpClientFactory httpClientFactory,
            CRMService crmService,
            IActividadUsuarioService actividadUsuarioService
        )
        {
            _unitOfWork = unitOfWork;
            _userIdentityService = userIdentityService;
            _httpClientFactory = httpClientFactory;
            _crmService = crmService;
            _actividadUsuarioService = actividadUsuarioService;
        }

        public async Task CreateUser(UserDto userDto)
        {
            var (rolSelected, empresasSelected) = ValidarUsuario(userDto);
            var createdId = await CrearUsuarioCRM(userDto);

            _unitOfWork
                .GetRepository<User>()
                .Insert(
                    new User
                    {
                        userName = userDto.Email,
                        nombre = userDto.Nombre,
                        apellido = userDto.Apellido,
                        telefono = userDto.Telefono,
                        estado = EstadosUsuario.activo,
                        isRDA = true,
                        idCRM = createdId,
                        Roles = [new() { rolId = rolSelected.id }],
                        EmpresasAsignaciones = empresasSelected
                            .Select(empresa => new UsuariosEmpresas { empresaId = empresa.id })
                            .ToList()
                    }
                );
            _unitOfWork.SaveChanges();

            _actividadUsuarioService.CrearActividadCrm(createdId, "Alta de nuevo usuario");
        }

        public async Task DesactivarUsuario(DesactivarConductorDto desactivarDto)
        {
            //Valido que el usuario del request puede editar al usuario target
            var maxJerarquiaRequest = _userIdentityService.GetJerarquiaRolMayor();
            var usersRepo = _unitOfWork.GetRepository<User>().GetAll();

            var user = usersRepo.Where(x => x.idCRM == desactivarDto.usuarioCrmId).FirstOrDefault();

            if (user == null)
                throw new BadRequestException("No se encontró el usuario a eliminar");

            var targetMaxJerarquia =
                user.Roles.Count() > 0 ? user.Roles.Max(x => x.Rol.jerarquia) : -1;
            if (targetMaxJerarquia >= maxJerarquiaRequest)
                throw new BadRequestException("No se poseen permisos para modificar este usuario");

            var httpClient = _httpClientFactory.CreateClient("CrmHttpClient");

            //Desasigno los autos del usuario
            var sinAsignarUserId = usersRepo
                .Where(x => x.nombre == "Sin" && x.apellido == "Asignar")
                .Select(x => x.idCRM)
                .FirstOrDefault();

            if (sinAsignarUserId == null)
                throw new BadRequestException(
                    "Error al desvincular los vehículos del usuario a desactivar"
                );

            foreach (
                var vehiculosMismoModulo in desactivarDto.vehiculosRelacionados.GroupBy(x =>
                    x.Tipo_de_Contrato
                )
            )
            {
                string criteriaFormat = "(id:equals:{0})";
                string concatenatedIds = string.Join(
                    " or ",
                    vehiculosMismoModulo.Select(id => string.Format(criteriaFormat, id))
                );
                string searchCriteria = $"criteria=({concatenatedIds})";

                //Busco los ID de los contratos internos a modificar
                var vehiculosIds = vehiculosMismoModulo.Select(x => x.id).Append(",");
                var contratosUrl = $"crm/v2/{vehiculosMismoModulo.Key}?fields=Id&" + searchCriteria;
                var contratosResponse = await _crmService.Get(contratosUrl);
                var contratosApiResponse = JsonSerializer.Deserialize<List<DeserializeIdDto>>(
                    contratosResponse
                );
                //Armo el JSON para actualizar ese modulo con los ID que recibi, y lo envio
                var desasignarVehiculosJson = new
                {
                    data = contratosApiResponse
                        .Select(register => new
                        {
                            id = register.id,
                            Conductor = new { id = sinAsignarUserId }
                        })
                        .ToArray()
                };

                var desasignarVehiculoJsonData = JsonSerializer.Serialize(desasignarVehiculosJson);
                var desasignarVehiculosContent = new StringContent(
                    desasignarVehiculoJsonData,
                    Encoding.UTF8,
                    "application/json"
                );
                var desasignarVehiculosResponse = await httpClient.PostAsync(
                    $"crm/v2/{vehiculosMismoModulo.Key}/upsert",
                    desasignarVehiculosContent
                );

                var desasignarResponse =
                    await desasignarVehiculosResponse.Content.ReadAsStringAsync();

                var desasignarApiResponse = JsonSerializer.Deserialize<ApiResponse>(
                    desasignarResponse
                );

                if (
                    desasignarApiResponse == null
                    || desasignarApiResponse.data == null
                    || desasignarApiResponse.data.Count == 0
                )
                    throw new BadRequestException("Respuesta inválida del CRM");

                var desasignarResponseData = desasignarApiResponse.data[0];
                if (desasignarResponseData.status != "success")
                {
                    throw new BadRequestException(
                        "Error al desasignar los vehiculos del usuario en el CRM"
                    );
                }
            }

            //Desactivo el usuario en el CRM y luego en la DB
            var jsonObj = new
            {
                data = new[]
                {
                    new { id = desactivarDto.usuarioCrmId, Estado_Mirai = EstadosUsuario.inactivo }
                }
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
                throw new BadRequestException("Error al editar el usuario en CRM");
            }

            user.estado = EstadosUsuario.inactivo;
            _unitOfWork.GetRepository<User>().Update(user);
            _unitOfWork.SaveChanges();

            _actividadUsuarioService.CrearActividadDb(user.id, "Desactivación del usuario");
        }

        public List<ConductorEmpresaDto> GetConductores()
        {
            var empresasDisponibles = _userIdentityService.ListarEmpresasDelUsuario();

            return
            [
                .. _unitOfWork
                    .GetRepository<User>()
                    .GetAll()
                    .Where(u =>
                        u.Roles.Any(reg => reg.Rol.nombreRol == "CONDUCTOR")
                        && u.EmpresasAsignaciones.Any(ea =>
                            empresasDisponibles.Contains(ea.Empresa.idCRM) && u.estado != "Inactivo"
                        )
                    )
                    .Select(x => new ConductorEmpresaDto
                    {
                        id = x.idCRM,
                        name = x.nombre + " " + x.apellido, //Mismo formato que otroga el CRM
                        empresaId = x.EmpresasAsignaciones.First().Empresa.idCRM,
                        estado = x.estado
                    })
            ];
        }

        public async Task<List<ConductorDto>>? GetListaUsuarios()
        {
            var empresasDisponibles = _userIdentityService.ListarEmpresasDelUsuario();
            var topeJerarquia = _userIdentityService.GetJerarquiaRolMayor();

            //Obtengo los datos necesarios
            var uri = new StringBuilder(
                "crm/v2/Contacts?fields=id,First_Name,Last_Name,Cargo,Phone,Email"
            );

            var json = await _crmService.Get(uri.ToString());
            var conductoresCrm = JsonSerializer.Deserialize<List<ConductorDto>>(json);

            //Get conductores de la DB y joineo con sus roles y empresas
            var conductoresDb = _unitOfWork
                .GetRepository<User>()
                .GetAll()
                .Where(x =>
                    conductoresCrm.Select(y => y.id).ToList().Contains(x.idCRM)
                    && x.Roles.All(x => x.Rol.jerarquia < topeJerarquia) //Si tiene 1 rol superior o igual, no va
                    && x.EmpresasAsignaciones.Any(x =>
                        empresasDisponibles.Contains(x.Empresa.idCRM)
                    )
                )
                .Select(x => new
                {
                    idCRM = x.idCRM,
                    Roles = x
                        .Roles.Select(x => new RolDto
                        {
                            Id = x.Rol.id,
                            NombreRol = x.Rol.nombreRol
                        })
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

            var vehiculosConductoresBag = new ConcurrentBag<ConductorVehiculoDto>();
            var conductoresCrmIds = conductoresPermisosMatch.Select(x => x.id).ToList();

            await Task.WhenAll(
                // Alquileres
                GetVehiculosPorUsuario(
                    "crm/v2/Alquileres?fields=",
                    ["Conductor", "Dominio_Alquiler"],
                    conductoresCrmIds,
                    vehiculosConductoresBag,
                    "Alquileres"
                ),
                // Servicios
                GetVehiculosPorUsuario(
                    "crm/v2/Servicios_RDA?fields=",
                    ["Conductor", "Dominio"],
                    conductoresCrmIds,
                    vehiculosConductoresBag,
                    "Servicios_RDA"
                ),
                // Renting
                GetVehiculosPorUsuario(
                    "crm/v2/Renting?fields=",
                    ["Conductor", "Dominio"],
                    conductoresCrmIds,
                    vehiculosConductoresBag,
                    "Renting"
                )
            );

            var vehiculosConductores = vehiculosConductoresBag.ToList();

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
                .FirstOrDefault(x => x.idCRM == id.ToString());
        }

        public async Task EditUser(string usuarioCrmId, UserDto userDto)
        {
            var (rolSelected, empresasSelected) = ValidarUsuario(userDto, usuarioCrmId);
            var usuarioEditarDb =
                _unitOfWork
                    .GetRepository<User>()
                    .GetAll()
                    .FirstOrDefault(x => x.idCRM == usuarioCrmId)
                ?? throw new NotFoundException("Usuario no encontrado");

            // no puedo modificar a un usuario con jerarquia igual o mayor
            var esInferior = _userIdentityService.EsInferiorEnRoles(usuarioEditarDb);
            if (!esInferior)
                throw new UnauthorizedAccessException(
                    "No puedes modificar a un usuario con jerarquía igual o mayor."
                );

            await ActualizarUsuarioCRM(usuarioCrmId, userDto);

            usuarioEditarDb.nombre = userDto.Nombre;
            usuarioEditarDb.apellido = userDto.Apellido;
            usuarioEditarDb.userName = userDto.Email;
            usuarioEditarDb.telefono = userDto.Telefono;

            ActualizarRoles(usuarioEditarDb, rolSelected);
            ActualizarEmpresas(usuarioEditarDb, empresasSelected);

            _unitOfWork.GetRepository<User>().Update(usuarioEditarDb);
            _unitOfWork.SaveChanges();

            _actividadUsuarioService.CrearActividadDb(
                usuarioEditarDb.id,
                "Edición de datos del usuario (terceros)"
            );
        }

        public async Task EditSelf(UpdateSelfUserDto userDto, string userName)
        {
            var user = _unitOfWork
                .GetRepository<User>()
                .GetAll()
                .FirstOrDefault(x => x.userName == userName);

            if (user == null || user.idCRM == null)
                throw new NotFoundException("Usuario no encontrado");

            // Actualizar el teléfono en el CRM
            await ActualizarTelefonoCRM(user.idCRM, userDto.Telefono);

            user.telefono = userDto.Telefono;
            _unitOfWork.GetRepository<User>().Update(user);
            _unitOfWork.SaveChanges();

            _actividadUsuarioService.CrearActividadDb(
                user.id,
                "Edición de datos del usuario (propios)"
            );
        }

        // Usuarios Empresas
        public List<UsuariosEmpresas>? GetAllUsuariosEmpresas() =>
            [.. _unitOfWork.GetRepository<UsuariosEmpresas>().GetAll()];

        public List<Empresa?> GetEmpresasDelUsuario(int usuarioId) =>
            [
                .. _unitOfWork
                    .GetRepository<UsuariosEmpresas>()
                    .GetAll()
                    .Where(x => x.userId == usuarioId)
                    .Select(x => x.Empresa)
            ];

        public UsuariosEmpresas? GetUsuariosEmpresasById(int id) =>
            _unitOfWork.GetRepository<UsuariosEmpresas>().GetById(id);

        // Usuarios Roles
        public List<UsuariosRoles>? GetAllUsuariosRoles() =>
            [.. _unitOfWork.GetRepository<UsuariosRoles>().GetAll()];

        private async Task GetVehiculosPorUsuario(
            string uri,
            string[] fields,
            List<string> conductoresIds,
            ConcurrentBag<ConductorVehiculoDto> vehiculosConductores,
            string tipoContrato
        )
        {
            var dataUri = new StringBuilder(uri);
            foreach (var field in fields)
                dataUri.Append(field).Append(",");

            var jsonData = await _crmService.Get(dataUri.ToString().TrimEnd(','));
            var dataArray = JArray.Parse(jsonData);

            foreach (var item in dataArray)
            {
                if (!item[fields[0]].HasValues || !item[fields[1]].HasValues)
                    continue;

                var conductor = item[fields[0]].ToObject<CRMRelatedObject>();
                if (!conductoresIds.Any(c => c == conductor.id))
                    continue;

                var dominio = item[fields[1]].ToObject<VehiculoRelacionadoDto>();
                dominio.Tipo_de_Contrato = tipoContrato;

                vehiculosConductores.Add(
                    new ConductorVehiculoDto { conductorCrmId = conductor.id, vehiculo = dominio, }
                );
            }
        }

        /// <summary>
        /// Valida que el usuario a crear sea válido
        /// </summary>
        /// <param name="userDto"></param>
        /// <param name="usuarioCrmId"></param>
        /// <exception cref="BadRequestException"></exception>
        private (Rol rol, List<Empresa> empresas) ValidarUsuario(
            UserDto userDto,
            string usuarioCrmId = ""
        )
        {
            //  El puesto tiene que estar dentro de OpcionesCargos
            var isPuestoExists = _unitOfWork
                .GetRepository<OpcionesCargos>()
                .GetAll()
                .Any(x => x.Nombre == userDto.Puesto);
            if (!isPuestoExists)
                throw new BadRequestException("El puesto no es válido");

            var isUserExists = _unitOfWork
                .GetRepository<User>()
                .GetAll()
                .Any(x => x.userName == userDto.Email && x.idCRM != usuarioCrmId);
            if (isUserExists)
                throw new BadRequestException("El correo electrónico ya está en uso");
            // el rolId tiene que ser un rol válido
            var rolSelected =
                _unitOfWork.GetRepository<Rol>().GetAll().FirstOrDefault(x => x.id == userDto.RolId)
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
            var rolesInferiores = _userIdentityService.ListarRolesInferiores();
            if (!rolesInferiores.Any(x => x.id == rolSelected.id))
                throw new BadRequestException("No tienes permisos para asignar ese rol");
            // si sos conductor solo podes tener una empresa asignada
            if (rolSelected.nombreRol == "CONDUCTOR" && empresasSelected.Count > 1)
                throw new BadRequestException("Un conductor solo puede tener una empresa asignada");
            // el usuario actual que crea, debe tener control sobre esas empresas
            var empresasDisponiblesSegunPermiso = _userIdentityService.ListarEmpresasDelUsuario();
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
        private async Task<string> CrearUsuarioCRM(UserDto userDto)
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
                        Comentario = "cargado desde plataforma",
                        Estado_Mirai = "Activo"
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
            var createdId =
                (responseData?.details?.id)
                ?? throw new BadRequestException(
                    "Error al crear el usuario en CRM, no se obtuvo el id"
                );
            return createdId;
        }

        private async Task ActualizarUsuarioCRM(string usuarioCrmId, UserDto userDto)
        {
            var httpClient = _httpClientFactory.CreateClient("CrmHttpClient");

            var jsonObj = new
            {
                data = new[]
                {
                    new
                    {
                        id = usuarioCrmId,
                        First_Name = userDto.Nombre,
                        Last_Name = userDto.Apellido,
                        Email = userDto.Email,
                        Cargo = userDto.Puesto,
                        Phone = userDto.Telefono,
                        Comentario = "actualizado desde plataforma"
                    }
                }
            };
            var jsonData = JsonSerializer.Serialize(jsonObj);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await httpClient.PutAsync($"crm/v2/Contacts/{usuarioCrmId}", content);
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
                        throw new BadRequestException("Error al actualizar el usuario en CRM");
                }
            }
        }

        private async Task ActualizarTelefonoCRM(string idCRM, string nuevoTelefono)
        {
            var httpClient = _httpClientFactory.CreateClient("CrmHttpClient");

            var jsonObj = new { data = new[] { new { Phone = nuevoTelefono } } };
            var jsonData = JsonSerializer.Serialize(jsonObj);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await httpClient.PutAsync($"crm/v2/Contacts/{idCRM}", content);
            var responseString = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseString);

            if (apiResponse == null || apiResponse.data == null || apiResponse.data.Count == 0)
                throw new BadRequestException("Respuesta inválida del CRM");

            var responseData = apiResponse.data[0];
            if (responseData.status != "success")
                throw new BadRequestException("Error al actualizar el teléfono en CRM");
        }

        private void ActualizarEmpresas(User usuarioEditarDb, List<Empresa> empresasSelected)
        {
            var usuarioEmpresasRepo = _unitOfWork.GetRepository<UsuariosEmpresas>();

            // Empresas actuales del usuario
            var empresasActuales = usuarioEditarDb
                .EmpresasAsignaciones.Select(e => e.empresaId)
                .ToList();

            // Empresas deseadas
            var empresasDeseadas = empresasSelected.Select(e => e.id).ToList();

            // Eliminar empresas que sobran
            var empresasAEliminar = empresasActuales.Except(empresasDeseadas).ToList();
            foreach (var empresaId in empresasAEliminar)
            {
                var empresaAEliminar = usuarioEditarDb.EmpresasAsignaciones.FirstOrDefault(e =>
                    e.empresaId == empresaId
                );
                if (empresaAEliminar != null)
                {
                    usuarioEmpresasRepo.Delete(empresaAEliminar);
                }
            }

            // Agregar empresas que faltan
            var empresasAAgregar = empresasDeseadas.Except(empresasActuales).ToList();
            foreach (var empresaId in empresasAAgregar)
            {
                usuarioEditarDb.EmpresasAsignaciones.Add(
                    new UsuariosEmpresas { empresaId = empresaId }
                );
            }
        }

        private void ActualizarRoles(User usuarioEditarDb, Rol rolSelected)
        {
            var usuariosRolesRepo = _unitOfWork.GetRepository<UsuariosRoles>();

            // Obtener rol actual del usuario
            var rolActual = usuarioEditarDb.Roles.First();

            if (rolActual.rolId != rolSelected.id)
            {
                // Eliminar rol actual
                var rolAEliminar = usuarioEditarDb.Roles.First();
                usuariosRolesRepo.Delete(rolAEliminar);

                // Agregar nuevo rol
                usuarioEditarDb.Roles.Add(new UsuariosRoles { rolId = rolSelected.id });
            }
        }
    }
}
