using System.Runtime.CompilerServices;
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
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace api.Controllers;

[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IRdaUnitOfWork _unitOfWork;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IUserIdentityService _userIdentityService;
    private readonly CRMService _crmService;

    public UsersController(
        CRMService crmService,
        IRdaUnitOfWork unitOfWork,
        IHttpClientFactory httpClientFactory,
        IUserIdentityService identityService
    )
    {
        _unitOfWork = unitOfWork;
        _httpClientFactory = httpClientFactory;
        _userIdentityService = identityService;
        _crmService = crmService;
    }

    [HttpGet]
    [Route("getPuestosOptions")]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN")]
    public IActionResult GetPuestos()
    {
        return Ok(CargoOptions.OpcionesValidas);
    }

    [HttpGet]
    [Route("GetListaUsuarios")]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN,CONDUCTOR")]
    public async Task<IActionResult> GetListaUsuarios()
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

        var result = conductoresPermisosMatch
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
                        x.ConductorPermiso.VehiculosAsignados.Add(v.vehiculo);
                    return x.ConductorPermiso;
                }
            )
            .Distinct()
            .ToList();

        return Ok(result);
    }

    private async Task GetVehiculosPorUsuario(
        string uri,
        string[] fields,
        List<string> conductoresIds,
        List<ConductorVehiculoDto> vehiculosConductores
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

            var dominio = item[fields[1]].ToObject<CRMRelatedObject>();

            vehiculosConductores.Add(
                new ConductorVehiculoDto { conductorCrmId = conductor.id, vehiculo = dominio }
            );
        }
    }

    [HttpGet]
    [Route("GetConductores")]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN")]
    public IActionResult GetConductores()
    {
        var empresasDisponibles = _userIdentityService.ListarEmpresasDelUsuario(User);

        var conductores = _unitOfWork
            .GetRepository<User>()
            .GetAll()
            .Where(u =>
                u.Roles.Any(reg => reg.Rol.nombreRol == "CONDUCTOR")
                && u.EmpresasAsignaciones.Any(ea => empresasDisponibles.Contains(ea.Empresa.idCRM))
            )
            .Select(x => new
            {
                id = x.idCRM,
                name = x.nombre + " " + x.apellido, //Mismo formato que otroga el CRM
                empresaId = x.EmpresasAsignaciones.First().Empresa.idCRM
            })
            .ToList();

        return Ok(conductores);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN")]
    public IActionResult GetById([FromRoute] int id)
    {
        var user = _unitOfWork
            .GetRepository<User>()
            .GetAll()
            .Where(x => x.idCRM == id.ToString())
            .SingleOrDefault();

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPost("DesactivarUsuario/{usuarioCrmId}")]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN")]
    public async Task<IActionResult> DesactivarUsuario(string usuarioCrmId)
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
        var response = await httpClient.PatchAsync($"crm/v2/Contacts/upsert", content);
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
        _unitOfWork.SaveChanges();

        return Ok();
    }

    [HttpPost]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto userDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
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
                    estado = EstadosUsuario.activo,
                    isRDA = true,
                    idCRM = createdId,
                    Roles = new List<UsuariosRoles> { new() { rolId = rolSelected.id } },
                    EmpresasAsignaciones = empresasSelected
                        .Select(empresa => new UsuariosEmpresas { empresaId = empresa.id })
                        .ToList()
                }
            );
        _unitOfWork.SaveChanges();

        return Created();
    }

    [HttpPost]
    [Route("editSelfConductor")]
    [Authorize(Roles = "CONDUCTOR")]
    public async Task<IActionResult> EditSelfConductor(
        [FromBody] UpdateSelfConductorDto conductorDto
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userName = User?.Identity?.Name;
        if (userName == null)
            return BadRequest("No se pudo obtener el id del usuario actual");
        var user = _unitOfWork
            .GetRepository<User>()
            .GetAll()
            .FirstOrDefault(x => x.userName == userName);
        if (user == null || user.idCRM == null)
            return NotFound("Usuario no encontrado");

        // Actualizar el teléfono en el CRM
        await ActualizarTelefonoCRM(user.idCRM, conductorDto.Telefono);

        // Actualizar el teléfono en la base de datos local
        user.telefono = conductorDto.Telefono;
        _unitOfWork.GetRepository<User>().Update(user);
        _unitOfWork.SaveChanges();

        return Ok("Teléfono actualizado correctamente");
    }

    /// <summary>
    /// Valida que el usuario a crear sea válido
    /// </summary>
    /// <param name="userDto"></param>
    /// <exception cref="BadRequestException"></exception>
    private (Rol rol, List<Empresa> empresas) ValidarUsuario(CreateUserDto userDto)
    {
        var isUserExists = _unitOfWork
            .GetRepository<User>()
            .GetAll()
            .Any(x => x.userName == userDto.Email);
        if (isUserExists)
            throw new BadRequestException("El correo electrónico ya está en uso");
        // el rolId tiene que ser un rol válido
        var rolSelected = _unitOfWork
            .GetRepository<Rol>()
            .GetAll()
            .SingleOrDefault(x => x.id == userDto.RolId);
        if (rolSelected == null)
            throw new BadRequestException("El rol seleccionado no es válido");
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

        var createdId = responseData?.details?.id;
        if (createdId == null)
            throw new BadRequestException("Error al crear el usuario en CRM, no se obtuvo el id");
        return createdId;
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

    private class ResponseData
    {
        public string? code { get; set; }
        public Details? details { get; set; }
        public string? message { get; set; }
        public string? status { get; set; }
    }

    private class Details
    {
        public string? id { get; set; }
    }

    private class ApiResponse
    {
        public List<ResponseData>? data { get; set; }
    }
}
