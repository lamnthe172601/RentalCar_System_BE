using Microsoft.AspNetCore.Mvc;
using RentalCar_System.Business.NotificationService;

namespace RentalCar_System.WebAPI.Controllers
{
   [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public EmailController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Subject) || string.IsNullOrEmpty(request.Message))
            {
                return BadRequest("Email, Subject, and Message are required.");
            }

            await _notificationService.SendEmailNotificationAsync(request.Email, request.Subject, request.Message);
            return Ok("Email sent successfully.");
        }
    }

    public class EmailRequest
    {
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}
