using api.DataAccess;
using api.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/[controller]")]
[ApiController]
// [Authorize(Roles = "RDA,SUPERADMIN,ADMIN")]
public class OpcionesCargosController(IRdaUnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet]
    public IActionResult GetPuestos()
    {
        var puestos = unitOfWork
            .GetRepository<OpcionesCargos>()
            .GetAll()
            .Select(x => x.Nombre)
            .ToList();
        return Ok(puestos);
    }
}
