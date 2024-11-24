using Microsoft.AspNetCore.Mvc;
using RentalCar_System.Business.PaymentService;

namespace RentalCar_System.API.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public AdminController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("statistics/total-revenue")]
        public async Task<IActionResult> GetTotalRevenue()
        {
            var totalRevenue = await _paymentService.GetTotalRevenueAsync();
            return Ok(new { TotalRevenue = totalRevenue });
        }

        [HttpGet("statistics/payment-count")]
        public async Task<IActionResult> GetPaymentCount([FromQuery] string status)
        {
            var count = await _paymentService.GetPaymentCountByStatusAsync(status);
            return Ok(new { Status = status, Count = count });
        }

        [HttpGet("statistics/payments")]
        public async Task<IActionResult> GetPaymentsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var payments = await _paymentService.GetPaymentsByDateRangeAsync(startDate, endDate);
            return Ok(payments);
        }
    }
}
