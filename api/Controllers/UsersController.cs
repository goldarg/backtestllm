using System.Text;
using System.Text.Json;
using api.Connected_Services;
using api.DataAccess;
using api.Exceptions;
using api.Models.DTO;
using api.Models.DTO.Conductor;
using api.Models.DTO.Empresa;
using api.Models.DTO.Rol;
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
        var topeJerarquia = _identityService.ListarRolesSuperiores(User)
            .Max(x => x.jerarquia);

        //Obtengo los datos necesarios
        var uri = new StringBuilder("crm/v2/Contacts?fields=id,Full_Name,Cargo");

        var json = await _crmService.Get(uri.ToString());
        var conductoresCrm = JsonSerializer.Deserialize<List<ConductorDto>>(json);

        //Get conductores de la DB y joineo con sus roles y empresas
        var conductoresDb = _unitOfWork.GetRepository<User>().GetAll()
            .Where(x => conductoresCrm.Select(y => y.id).ToList().Contains(x.idCRM)
                && x.Roles.All(x => x.Rol.jerarquia < topeJerarquia) //Si tiene 1 rol superior o igual, no va
                && x.EmpresasAsignaciones.Any(x => empresasDisponibles.Contains(x.Empresa.idCRM))
            )
            .Select(x => new {
                idCRM = x.idCRM,
                Roles = x.Roles.Select(x => new RolDto
                {
                    Id = x.Rol.id,
                    NombreRol = x.Rol.nombreRol
                }).ToList(),
                Empresas = x.EmpresasAsignaciones.Select(x => new EmpresaDto
                {
                    IdCRM = x.Empresa.idCRM,
                    RazonSocial = x.Empresa.razonSocial
                }).ToList()
            })
            .ToList();

        //Con este "inner join" estoy filtrando del CRM los users que no tienen la empresa o roles
        var conductoresPermisosMatch = conductoresCrm.Join(conductoresDb, crm => crm.id, db => db.idCRM, (crm, db) => 
        {
            crm.Roles = db.Roles;
            crm.Empresas = db.Empresas;
            crm.Estado = "Placeholder";
            return crm;
        }).ToList();

        var vehiculosConductores = new List<ConductorVehiculoDto>();
        var conductoresCrmIds = conductoresPermisosMatch.Select(x => x.id).ToList();

        await Task.WhenAll(
            // Alquileres
            GetVehiculosPorUsuario("crm/v2/Alquileres?fields=", ["Conductor", "Dominio_Alquiler"], conductoresCrmIds, vehiculosConductores),
            // Servicios
            GetVehiculosPorUsuario("crm/v2/Servicios_RDA?fields=", ["Conductor", "Dominio"], conductoresCrmIds, vehiculosConductores),
            // Renting
            GetVehiculosPorUsuario("crm/v2/Renting?fields=", ["Conductor", "Dominio"], conductoresCrmIds, vehiculosConductores)
        );

        var result = conductoresPermisosMatch.Join(vehiculosConductores, r => r.id, v => v.conductorCrmId, (r,v) => {
                r.VehiculosAsignados.Add(v.vehiculo);
            return r;
        }).ToList();

        return Ok(result);
    }

    private async Task GetVehiculosPorUsuario(string uri, string[] fields, List<string> conductoresIds, List<ConductorVehiculoDto> vehiculosConductores)
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
            var conductor = item[fields[0]].ToObject<CRMRelatedObject>();
            if (!conductoresIds.Any(c => c == conductor.id))
                return;

            var dominio = item[fields[1]].ToObject<CRMRelatedObject>();

            vehiculosConductores.Add(new ConductorVehiculoDto
            {
                conductorCrmId = conductor.id,
                vehiculo = dominio
            });
        }
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