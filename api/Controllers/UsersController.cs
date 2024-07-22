using api.DataAccess;
using api.Exceptions;
using api.Models.DTO;
using api.Models.DTO.Empresa;
using api.Models.Entities;
using api.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IRdaUnitOfWork _unitOfWork;
    private readonly IUserIdentityService _identityService;

    public UsersController(IUserIdentityService identityService, IRdaUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var users = _unitOfWork.GetRepository<User>().GetAll()
            .ToList();

        return Ok(users);
    }

    [HttpGet]
    [Route("GetConductores")]
    public IActionResult GetConductores()
    {
        var empresasDisponibles = _identityService.ListarEmpresasDelUsuario(User);

        var conductores = _unitOfWork.GetRepository<User>().GetAll()
            .Where(u => u.Roles.Any(reg => reg.Rol.nombreRol == "CONDUCTOR") &&
            u.EmpresasAsignaciones.Any(ea => empresasDisponibles.Contains(ea.Empresa.idCRM)))
            .Select(x => new
            {
                id = x.idCRM,
                name = x.nombre + " " + x.apellido, //Mismo formato que otroga el CRM
                empresaId = x.EmpresasAsignaciones.First().empresaId
            })
            .ToList();

        return Ok(conductores);
    }

    [HttpGet("{id}")]
    public IActionResult GetById([FromRoute] int id)
    {
        var user = _unitOfWork.GetRepository<User>().GetAll().Where(x => x.idCRM == id.ToString()).SingleOrDefault();

        if (user == null)
            return NotFound();

        return Ok(user);
    }
}