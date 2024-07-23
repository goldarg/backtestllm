using api.DataAccess;
using api.Models.DTO;
using api.Models.DTO.Empresa;
using api.Models.Entities;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/empresas")]
[ApiController]
public class EmpresasController : ControllerBase
{
    private readonly IRdaUnitOfWork _unitOfWork;
    private readonly IUserIdentityService _userIdentityService;

    public EmpresasController(IRdaUnitOfWork unitOfWork, IUserIdentityService userIdentityService)
    {
        _unitOfWork = unitOfWork;
        _userIdentityService = userIdentityService;
    }

    [HttpGet]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN")]
    public IActionResult GetAll()
    {
        var empresasUsuario = _userIdentityService.ListarEmpresasDelUsuario(User);

        var empresas = _unitOfWork.GetRepository<Empresa>().GetAll()
            .Where(x => empresasUsuario.Contains(x.idCRM))
            .Select(x => new EmpresaDto
            {
                IdCRM = x.idCRM,
                RazonSocial = x.razonSocial
            })
            .ToList();

        return Ok(empresas);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN")]
    public IActionResult GetById([FromRoute] int id)
    {
        var empresa = _unitOfWork.GetRepository<Empresa>().GetById(id);

        if (empresa == null)
            return NotFound();

        return Ok(empresa);
    }
}