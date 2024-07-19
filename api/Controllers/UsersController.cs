using api.DataAccess;
using api.Models.DTO;
using api.Models.DTO.Conductor;
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
            .Select(e => new ConductorDto
            {
                id = e.id,
                idCRM = e.idCRM,
                nombre = e.nombre,
                apellido = e.apellido,
                userName = e.userName,
                Empresa = new EmpresaDto { //Por definición, un conductor siempre tiene 1 empresa asociada
                    id = e.EmpresasAsignaciones.First().Empresa.id,
                    idCRM = e.EmpresasAsignaciones.First().Empresa.idCRM,
                    razonSocial = e.EmpresasAsignaciones.First().Empresa.razonSocial
                }
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