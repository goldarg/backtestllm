using api.DataAccess;
using api.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/usuariosEmpresas")]
[ApiController]
public class UsuariosEmpresasController : ControllerBase
{
    private readonly IRdaUnitOfWork _unitOfWork;

    public UsuariosEmpresasController(IRdaUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var usuariosEmpresas = _unitOfWork.GetRepository<UsuariosEmpresas>().GetAll()
            .ToList();
        return Ok(usuariosEmpresas);
    }

    [HttpGet("{id}")]
    public IActionResult GetById([FromRoute] int id)
    {
        var usuariosEmpresas = _unitOfWork.GetRepository<UsuariosEmpresas>().GetById(id);

        if (usuariosEmpresas == null)
            return NotFound();

        return Ok(usuariosEmpresas);
    }
}