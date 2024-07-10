using api.DataAccess;
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

    [HttpGet("{id}")]
    public IActionResult GetById([FromRoute] int id)
    {
        var user = _unitOfWork.GetRepository<User>().GetById(id);

        if (user == null)
            return NotFound();

        return Ok(user);
    }
}