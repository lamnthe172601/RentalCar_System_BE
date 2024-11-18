using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentalCar_System.Business.CartService;
using RentalCar_System.Business.VnPayLibrary;
using RentalCar_System.Models.DtoViewModel;
using System.Globalization;

namespace RentalCar_System.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IConfiguration configuration, ILogger<PaymentController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("create-payment")]
        public IActionResult CreatePayment([FromBody] VnPayRequestModel model)
        {
            try
            {
                if (model == null || string.IsNullOrEmpty(model.OrderId) || model.Amount <= 0)
                {
                    return BadRequest("Invalid payment request data.");
                }

                var vnpay = new VnPayLibrary();
                var tmnCode = _configuration["Vnpay:TmnCode"];
                var hashSecret = _configuration["Vnpay:HashSecret"];
                var baseUrl = _configuration["Vnpay:BaseUrl"];
                var returnUrl = _configuration["Vnpay:ReturnUrl"];
                var tick = DateTime.Now.Ticks.ToString();
                string clientIPAddress = Utils.GetIpAddress(HttpContext);

                vnpay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
                vnpay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
                vnpay.AddRequestData("vnp_TmnCode", tmnCode);
                vnpay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString(CultureInfo.InvariantCulture));
                vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_CurrCode", _configuration["VnPay:CurrCode"]);
                vnpay.AddRequestData("vnp_IpAddr", clientIPAddress);
                vnpay.AddRequestData("vnp_ExpireDate", DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_Locale", _configuration["VnPay:Locale"]);
                vnpay.AddRequestData("vnp_OrderInfo", model.OrderInfo ?? $"Order {model.OrderId}");
                vnpay.AddRequestData("vnp_OrderType", "VNPAY");
                vnpay.AddRequestData("vnp_ReturnUrl", returnUrl);
                vnpay.AddRequestData("vnp_TxnRef", tick);
                vnpay.AddRequestData("vnp_BankCode", "NCB");

                var paymentUrl = vnpay.CreateRequestUrl(baseUrl, hashSecret);

                _logger.LogInformation("VNPAY payment URL created: {Url}", paymentUrl);

                return Ok(new { Url = paymentUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating payment URL: {Message}", ex.Message);
                return StatusCode(500, "An error occurred while creating the payment URL.");
            }
        }


        [HttpGet("return")]
        public IActionResult PaymentReturn()
        {
            try
            {
                var vnpay = new VnPayLibrary();
                var hashSecret = _configuration["Vnpay:HashSecret"];

                // Lấy dữ liệu từ callback
                foreach (var (key, value) in Request.Query)
                {
                    vnpay.AddResponseData(key, value);
                }

                // Xác minh chữ ký
                if (!vnpay.ValidateSignature(Request.Query["vnp_SecureHash"], hashSecret))
                {
                    return BadRequest("Invalid signature.");
                }

                // Lấy trạng thái giao dịch
                string transactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                if (transactionStatus == "00")
                {
                    return Ok(new
                    {
                        Status = "Success",
                        Message = "Transaction successful.",
                        OrderId = vnpay.GetResponseData("vnp_TxnRef"),
                        Amount = vnpay.GetResponseData("vnp_Amount"),
                        PaymentDate = vnpay.GetResponseData("vnp_PayDate")
                    });
                }

                return BadRequest(new
                {
                    Status = "Failed",
                    Message = "Transaction failed.",
                    OrderId = vnpay.GetResponseData("vnp_TxnRef"),
                    Amount = vnpay.GetResponseData("vnp_Amount")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error processing VNPAY return: {Message}", ex.Message);
                return StatusCode(500, "An error occurred while processing the payment return.");
            }
        }


    }


    public class VnPayRequestModel
    {
        public string OrderId { get; set; } // Unique order identifier
        public decimal Amount { get; set; } // Payment amount
        public string OrderInfo { get; set; } // Optional: Order description
    }
}
