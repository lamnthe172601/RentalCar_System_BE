﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentalCar_System.Business.CarService;
using RentalCar_System.Business.CartService;
using RentalCar_System.Business.PaymentService;
using RentalCar_System.Business.RentalCarService;
using RentalCar_System.Business.VnPayLibrary;
using RentalCar_System.Models.DtoViewModel;
using RentalCar_System.Models.Entity;
using System.Globalization;

namespace RentalCar_System.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentController> _logger;
        private readonly IRentalContractService _rentalContractService;
        private readonly PaymentService _paymentService;
        private readonly ICartService _cartService;
        private readonly ICarService _carService;
        public PaymentController(IConfiguration configuration
            , ILogger<PaymentController> logger
            , IRentalContractService rentalContractService,
              PaymentService paymentService,
                                 ICartService cartService,
                                 ICarService carService)
        {
            _configuration = configuration;
            _logger = logger;
            _rentalContractService = rentalContractService;
            _paymentService = paymentService;
            _cartService = cartService;
            _carService = carService;
        }

        [HttpPost("create-payment")]
        public async  Task<IActionResult> CreatePayment([FromBody] VnPayRequestModel model)
        {
            try
            {
                if (model == null || string.IsNullOrEmpty(model.ContractId.ToString()) || model.Amount <= 0)
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
                vnpay.AddRequestData("vnp_OrderInfo", $"{model.ContractId}");
                vnpay.AddRequestData("vnp_OrderType", "VNPAY");
                vnpay.AddRequestData("vnp_ReturnUrl", returnUrl);
                vnpay.AddRequestData("vnp_TxnRef", tick);
                vnpay.AddRequestData("vnp_BankCode", "NCB");

                var paymentUrl = vnpay.CreateRequestUrl(baseUrl, hashSecret);

                _logger.LogInformation("VNPAY payment URL created: {Url}", paymentUrl);

                var rentalContract =  await _rentalContractService.GetRentalContractByIdAsync(model.ContractId);

                if (rentalContract == null)
                {
                    return NotFound("Rental contract not found.");
                }

                await _paymentService.AddPaymentAsync(new Payment
                {   UserId = rentalContract.UserId,  
                    ContractId = model.ContractId,
                    Amount = model.Amount,
                    PaymentDate = DateTime.Now,
                    Status = "Pending",
                });
                return Ok(new { Url = paymentUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating payment URL: {Message}", ex.Message);
                return StatusCode(500, "An error occurred while creating the payment URL.");
            }
        }


        [HttpGet("return")]
        public async Task<IActionResult> PaymentReturn()
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
                var contractId = Guid.Parse(vnpay.GetResponseData("vnp_OrderInfo").Trim());
                var sum = 99;
                // Lấy trạng thái giao dịch
                string transactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                if (transactionStatus == "00")
                {
                    await UpdatePaymentStatus(contractId, "Completed");
                    await _rentalContractService.UpdateContractStatusAsync(contractId, "Completed");
                    await _cartService.RemoveFromCartByContractIdAsync(contractId);
                    await _carService.UpdateStatusCar(contractId, "Rented");
                    return Ok(new
                    {
                        Status = "Success",
                        Message = "Transaction successful.",
                        ContractId = vnpay.GetResponseData("vnp_OrderInfo"),
                        Amount = vnpay.GetResponseData("vnp_Amount"),
                        PaymentDate = vnpay.GetResponseData("vnp_PayDate")
                    });
                }
                await UpdatePaymentStatus(contractId, "Fail");
                await _rentalContractService.UpdateContractStatusAsync(contractId, "Fail");
                return BadRequest(new
                {   
                    Status = "Failed",
                    Message = "Transaction failed.",
                    ContractId = vnpay.GetResponseData("vnp_OrderInfo"),
                    Amount = vnpay.GetResponseData("vnp_Amount")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error processing VNPAY return: {Message}", ex.Message);
                return StatusCode(500, "An error occurred while processing the payment return.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(Guid id)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null)
            {
                return NotFound();
            }
            return Ok(payment);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPayments()
        {
            var payments = await _paymentService.GetAllPaymentsAsync();
            return Ok(payments);
        }

        [HttpPost]
        public async Task<IActionResult> AddPayment([FromBody] PaymentDto model)
        {
            var payment = new Payment
            {
                PaymentId = Guid.NewGuid(),
                Amount = model.Amount,
                PaymentDate = DateTime.Now,
                Status = model.Status
            };
            await _paymentService.AddPaymentAsync(payment);
            return CreatedAtAction(nameof(GetPaymentById), new { id = payment.PaymentId }, payment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayment(Guid id, [FromBody] PaymentDto model)
        {

            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null)
            {
                return NotFound();
            }
            payment.PaymentId = payment.PaymentId;
            payment.Amount = model.Amount;
            payment.PaymentDate = payment.PaymentDate;
            payment.Status = model.Status;



            await _paymentService.UpdatePaymentAsync(payment);
            return NoContent();
        }

        [HttpPut("update-Status")]
        public async Task<IActionResult> UpdatePaymentStatus(Guid id,string status)
        {

            var payment = await _paymentService.GetPaymentByContractIdAsync(id);
            if (payment == null)
            {
                return NotFound();
            }          
            payment.Status = status;
            await _paymentService.UpdatePaymentAsync(payment);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(Guid id)
        {
            await _paymentService.DeletePaymentAsync(id);
            return NoContent();
        }


    }


    public class VnPayRequestModel
    {
        public Guid ContractId { get; set; } // Unique order identifier
        public decimal Amount { get; set; } // Payment amount      
    }
}
