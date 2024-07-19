using api.DataAccess;
using api.Models.DTO;
using api.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/empresas")]
[ApiController]
public class EmpresasController : ControllerBase
{
    private readonly IRdaUnitOfWork _unitOfWork;

    public EmpresasController(IRdaUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var empresas = _unitOfWork.GetRepository<Empresa>().GetAll()
            .Select(x => new CRMRelatedObject
            {
                id = x.idCRM,
                name = x.razonSocial
            })
            .ToList();

        return Ok(empresas);
    }

    [HttpGet("{id}")]
    public IActionResult GetById([FromRoute] int id)
    {
        var empresa = _unitOfWork.GetRepository<Empresa>().GetById(id);

        if (empresa == null)
            return NotFound();

        return Ok(empresa);
    }
}