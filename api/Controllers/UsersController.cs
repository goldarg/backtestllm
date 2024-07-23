using System.Text;
using System.Text.Json;
using api.Connected_Services;
using api.DataAccess;
using api.Exceptions;
using api.Models.DTO;
using api.Models.DTO.Conductor;
using api.Models.DTO.Empresa;
using api.Models.Entities;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IRdaUnitOfWork _unitOfWork;
    private readonly IUserIdentityService _identityService;
    private readonly CRMService _crmService;

    public UsersController(CRMService crmService, IUserIdentityService identityService, IRdaUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
        _crmService = crmService;
    }

    [HttpGet]
    [Route("GetListaUsuarios")]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN,CONDUCTOR")]
    public async Task<IActionResult> GetListaUsuarios()
    {
        var empresasDisponibles = _identityService.ListarEmpresasDelUsuario(User);
        var forbiddenRoles = _identityService.ListarRolesSuperiores(User);

        //Obtengo los datos necesarios
        var uri = new StringBuilder("crm/v2/Contacts?fields=id,Full_Name,Cargo");

        var json = await _crmService.Get(uri.ToString());
        var conductoresCrm = JsonSerializer.Deserialize<List<ConductorDto>>(json);

        var conductoresDb = _unitOfWork.GetRepository<User>().GetAll()
            .Where(x => conductoresCrm.Select(y => y.id).ToList().Contains(x.idCRM)
                && x.Roles.All(x => !forbiddenRoles.Contains(x.Rol)) //Si tiene 1 rol superior, no se muestra
            ).ToList();

        conductoresCrm.Join(conductoresDb, crm => crm.id, db => db.idCRM, (crm, db) => 
        {
            crm.Roles = 
        }).ToList();

        //Empresas de cada contacto
        //Roles de cada contacto

        //Filtro segun roles del usuario
            //Si es RDA saltea

        //Filtro segun empresas del usuario
            //Si es RDA saltea

        return Ok(3);
    }

    [HttpGet]
    [Route("GetConductores")]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN")]
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
}