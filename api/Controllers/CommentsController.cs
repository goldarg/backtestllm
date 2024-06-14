using api.Data;
using api.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/comments")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly IRdaUnitOfWork _unitOfWork;

        public CommentsController(IRdaUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var comment = _unitOfWork.GetRepository<Comment>().GetAll()
                .ToList();
            return Ok(comment);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var comment = _unitOfWork.GetRepository<Comment>().GetByID(id);

            if (comment == null)
                return NotFound();

            return Ok(comment);
        }
    }
}