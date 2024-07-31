using api.DataAccess;
using api.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/[controller]")]
    public class NotifyHubController : Controller
    {
        private readonly IRdaUnitOfWork _unitOfWork;
        
        public NotifyHubController(IRdaUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("ReturnData")]
        public IActionResult ReturnData([FromBody] dynamic notificationResponse)
        {
            var dataNotificationAPI = new DataNotificationAPI
            {
                Response = notificationResponse.ToString()
            };

            _unitOfWork.GetRepository<DataNotificationAPI>().Insert(dataNotificationAPI);

            _unitOfWork.SaveChanges();

            return Ok();
        }
    }
}