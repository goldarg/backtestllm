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
    private readonly IUserIdentityService _userIdentityService;

    public UsersController(IUserIdentityService identityService, IRdaUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _userIdentityService = identityService;
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
    // [Authorize(Roles = "RDA,SUPERADMIN,ADMIN")]
    public IActionResult CreateUser([FromBody] CreateUserDto userDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        // el rolId tiene que ser un rol válido
        var rolSelected = _unitOfWork.GetRepository<Rol>().GetAll().SingleOrDefault(x => x.id == userDto.RolId);
        if (rolSelected == null)
            throw new BadRequestException("El rol seleccionado no es válido");
        // las empresasIdsCrm tienen que ser empresas válidas
        var empresasSelected = _unitOfWork.GetRepository<Empresa>()
            .GetAll().Where(x => x.idCRM != null && userDto.EmpresasIdsCrm.Contains(x.idCRM))
            .Select(x => x.idCRM).ToList();
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
        if (!empresasSelected.All(x => empresasDisponiblesSegunPermiso.Contains(x)))
            throw new BadRequestException("No tienes permisos para asignar esas empresas");
        // el email no tiene que estar ya usado
        bool isUserExists = _unitOfWork.GetRepository<User>().GetAll().Any(x => x.userName == userDto.Email);
        if (isUserExists)
            throw new BadRequestException("El correo electrónico ya está en uso");

        // TODO sino sos conductor no podes tener un auto asignado


        return Ok(userDto);
    }
}