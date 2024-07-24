using System.Text;
using System.Text.Json;
using api.Configuration;
using api.DataAccess;
using api.Exceptions;
using api.Models.DTO;
using api.Models.DTO.Empresa;
using api.Models.DTO.User;
using api.Models.Entities;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IRdaUnitOfWork _unitOfWork;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IUserIdentityService _userIdentityService;

    public UsersController(IRdaUnitOfWork unitOfWork, IHttpClientFactory httpClientFactory, IUserIdentityService identityService)
    {
        _unitOfWork = unitOfWork;
        _httpClientFactory = httpClientFactory;
        _userIdentityService = identityService;
    }

    [HttpGet]
    [Route("getPuestosOptions")]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN")]
    public IActionResult GetPuestos()
    {
        return Ok(CargoOptions.OpcionesValidas);
    }

    [HttpGet]
    [Route("GetConductores")]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN")]
    public IActionResult GetConductores()
    {
        var empresasDisponibles = _userIdentityService.ListarEmpresasDelUsuario(User);

        var conductores = _unitOfWork.GetRepository<User>().GetAll()
            .Where(u => u.Roles.Any(reg => reg.Rol.nombreRol == "CONDUCTOR") &&
            u.EmpresasAsignaciones.Any(ea => empresasDisponibles.Contains(ea.Empresa.idCRM)))
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
        var user = _unitOfWork.GetRepository<User>().GetAll()
        .Where(x => x.idCRM == id.ToString())
        .SingleOrDefault();

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPost]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto userDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var (rolSelected, empresasSelected) = ValidarUsuario(userDto);
        var createdId = await CrearUsuarioCRM(userDto);

        _unitOfWork.GetRepository<User>().Insert(new User
        {
            userName = userDto.Email,
            nombre = userDto.Nombre,
            apellido = userDto.Apellido,
            activo = true,
            isRDA = true,
            idCRM = createdId,
            Roles = new List<UsuariosRoles>
            {
                new UsuariosRoles
                {
                    rolId = rolSelected.id
                }
            },
            EmpresasAsignaciones = empresasSelected.Select(empresa => new UsuariosEmpresas
            {
                empresaId = empresa.id
            }).ToList()
        });
        _unitOfWork.SaveChanges();

        return Created();
    }

    /// <summary>
    /// Valida que el usuario a crear sea válido
    /// </summary>
    /// <param name="userDto"></param>
    /// <exception cref="BadRequestException"></exception>
    private (Rol rol, List<Empresa> empresas) ValidarUsuario(CreateUserDto userDto)
    {
        bool isUserExists = _unitOfWork.GetRepository<User>().GetAll().Any(x => x.userName == userDto.Email);
        if (isUserExists)
            throw new BadRequestException("El correo electrónico ya está en uso");
        // el rolId tiene que ser un rol válido
        var rolSelected = _unitOfWork.GetRepository<Rol>().GetAll().SingleOrDefault(x => x.id == userDto.RolId);
        if (rolSelected == null)
            throw new BadRequestException("El rol seleccionado no es válido");
        // las empresasIdsCrm tienen que ser empresas válidas
        var empresasSelected = _unitOfWork.GetRepository<Empresa>()
            .GetAll().Where(x => x.idCRM != null && userDto.EmpresasIdsCrm.Contains(x.idCRM))
            .ToList();
        if (empresasSelected.Count != userDto.EmpresasIdsCrm.Count)
            throw new BadRequestException("Al menos una de las empresas seleccionadas no es válida");
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
                    throw new BadRequestException("Ya existe un usuario en el CRM con ese correo electrónico");
                default:
                    // TODO habria que loggear responseData.message
                    throw new BadRequestException("Error al crear el usuario en CRM");
            }
        }
        var createdId = responseData?.details?.id;
        if (createdId == null)
            throw new BadRequestException("Error al crear el usuario en CRM, no se obtuvo el id");
        return createdId;
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