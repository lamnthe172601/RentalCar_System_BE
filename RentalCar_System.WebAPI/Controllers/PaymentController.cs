using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentalCar_System.Business.CartService;
using RentalCar_System.Business.VnPayLibrary;
using RentalCar_System.Business.VnPayLibrary.RentalCar_System.Business.VnPayLibrary;
using RentalCar_System.Models.DtoViewModel;

namespace RentalCar_System.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IConfiguration _configuration;

        public PaymentController(ICartService cartService, IConfiguration configuration)
        {
            _cartService = cartService;
            _configuration = configuration;
        }

        [HttpPost("vnpay")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentRequestDto paymentRequest)
        {
            var cartItems = await _cartService.GetCartItemsAsync(paymentRequest.UserId);
            var totalAmount = cartItems.Sum(item => item.Price);

            string vnp_TmnCode = _configuration["VNPay:vnp_TmnCode"];
            string vnp_HashSecret = _configuration["VNPay:vnp_HashSecret"];
            string vnp_Url = _configuration["VNPay:vnp_Url"];
            string vnp_ReturnUrl = _configuration["VNPay:vnp_ReturnUrl"];

            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (totalAmount * 100).ToString()); // Amount in smallest currency unit
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString());
            vnpay.AddRequestData("vnp_OrderInfo", "Payment for rental car");
            vnpay.AddRequestData("vnp_OrderType", "billpayment");
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_ReturnUrl);
            vnpay.AddRequestData("vnp_IpAddr", Request.HttpContext.Connection.RemoteIpAddress.ToString());

            // Log the request data
            var requestData = vnpay.GetRequestData();
            foreach (var data in requestData)
            {
                Console.WriteLine($"{data.Key}: {data.Value}");
            }

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

            return Ok(new { paymentUrl });
        }


        [HttpGet("vnpay_return")]
        public IActionResult PaymentReturn()
        {
            var vnpayData = Request.Query;
            var vnpay = new VnPayLibrary();

            foreach (var (key, value) in vnpayData)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value);
                }
            }

            string vnp_HashSecret = _configuration["VNPay:vnp_HashSecret"];
            bool isValidSignature = vnpay.ValidateSignature(vnpayData, vnp_HashSecret);

            if (isValidSignature)
            {
                // Handle payment success
                return Ok(new { message = "Payment successful" });
            }
            else
            {
                // Handle payment failure
                return BadRequest(new { message = "Invalid signature" });
            }
        }
    }
}
