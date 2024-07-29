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

    [HttpPost]
    public IActionResult PostPuesto([FromBody] string puesto)
    {
        // check que no se repita
        var isPuestoExists = unitOfWork
            .GetRepository<OpcionesCargos>()
            .GetAll()
            .Any(x => x.Nombre == puesto);
        if (isPuestoExists)
            return BadRequest("El puesto ya existe");
        var puestoEntity = new OpcionesCargos { Nombre = puesto };
        unitOfWork.GetRepository<OpcionesCargos>().Insert(puestoEntity);
        unitOfWork.SaveChanges();
        return Ok();
    }

    // borrar recibe el nombre
    [HttpDelete]
    public IActionResult DeletePuesto([FromBody] string puesto)
    {
        var puestoEntity = unitOfWork
            .GetRepository<OpcionesCargos>()
            .GetAll()
            .FirstOrDefault(x => x.Nombre == puesto);
        if (puestoEntity == null)
            return BadRequest("El puesto no existe");
        unitOfWork.GetRepository<OpcionesCargos>().Delete(puestoEntity);
        unitOfWork.SaveChanges();
        return Ok();
    }
}
