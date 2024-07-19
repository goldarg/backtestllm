using api.DataAccess;
using api.Models.DTO;
using api.Models.DTO.Empresa;
using api.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IRdaUnitOfWork _unitOfWork;

    public UsersController(IRdaUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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
        var userId = User.Identity.Name; //TODO ver de donde sale el username o el ID
        var placeholder = 3;
        var requestUser = _unitOfWork.GetRepository<User>().GetAll()
            .Where(x => x.id == placeholder)
            .SingleOrDefault();

        if (requestUser == null)
            throw new Exception ("No se encontró el usuario solicitante");
        
        var empresasDisponibles = requestUser.EmpresasAsignaciones.Select(x => x.empresaId).ToList();

        var conductores = _unitOfWork.GetRepository<User>().GetAll()
            .Where(u => u.Roles.Any(reg => reg.Rol.nombreRol == "CONDUCTOR") &&
            u.EmpresasAsignaciones.Any(ea => empresasDisponibles.Contains(ea.empresaId)))
            .Select(x => new {
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