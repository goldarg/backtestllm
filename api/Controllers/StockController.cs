using api.DataAccess;
using api.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/stock")]
[ApiController]
public class StockController : ControllerBase
{
    private readonly IRdaUnitOfWork _unitOfWork;

    public StockController(IRdaUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var stocks = _unitOfWork.GetRepository<Stock>().GetAll()
            .ToList();

        return Ok(stocks);
    }

    [HttpGet("{id}")]
    public IActionResult GetById([FromRoute] int id)
    {
        var stock = _unitOfWork.GetRepository<Stock>().GetById(id);

        if (stock == null)
            return NotFound();

        return Ok(stock);
    }
}