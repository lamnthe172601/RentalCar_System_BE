﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentalCar_System.Business.CarService;
using RentalCar_System.Business.RentalCarService;
using RentalCar_System.Models.DtoViewModel;
using RentalCar_System.Models.Entity;

namespace RentalCar_System.WebAPI.Controllers
{
    [Route("api/[controller]")]
    

    [ApiController]
    public class RentalContractsController : ControllerBase
    {
        private readonly IRentalContractService _rentalContractService;
        public RentalContractsController(IRentalContractService rentalContractService)
        {
            _rentalContractService = rentalContractService;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<CarRented>>> GetRentalContractsByUserId(Guid userId)
        {
            var rentalContracts = await _rentalContractService.GetAllContractsByUserIdAsync(userId);
            if (rentalContracts == null || !rentalContracts.Any())
            {
                return NotFound();
            }
            return Ok(rentalContracts);
        }


        [HttpGet("{contractId}")]
        public async Task<ActionResult<RentalContract>> GetRentalContractById(Guid contractId)
        {
            try
            {
                var rentalContract = await _rentalContractService.GetRentalContractByIdAsync(contractId);

                if (rentalContract == null)
                {
                    return NotFound(new { Message = $"Rental contract with ID {contractId} not found." });
                }

                return Ok(rentalContract);
            }
            catch (Exception ex)
            {
               
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching data.");
            }
        }

        [HttpPost("request")]
        public async Task<IActionResult> SendRentRequest([FromBody] RentCarRequest request)
        {
            try
            {
                var rentalContract = await _rentalContractService.SendRentRequestAsync(request.UserId, request.CarId, request.RentalDate, request.ReturnDate);
                return CreatedAtAction(nameof(GetRentalContractById), new { contractId = rentalContract.ContractId }, rentalContract);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{contractId}/cancel")]
        public async Task<IActionResult> CancelRentalContract(Guid contractId)
        {
            try
            {
                
                var success = await _rentalContractService.CancelRentalContractAsync(contractId);

                if (success)
                {
                    return Ok(new { message = "Rental contract canceled successfully." });
                }

                return BadRequest(new { message = "Failed to cancel the rental contract." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{contractId}/feedback")]
        public async Task<IActionResult> SubmitFeedbackAndRating(Guid contractId, [FromBody] FeedbackRatingRequest request)
        {
            try
            {
                var success = await _rentalContractService.UpdateFeedbackAndRatingAsync(contractId, request.Feedback, request.Rating);
                if (success)
                {
                    return Ok(new { Message = "Feedback and rating submitted successfully." });
                }
                return BadRequest(new { Message = "Unable to submit feedback." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while submitting feedback.", Error = ex.Message });
            }
        }
        [HttpGet("notify-expiring-contracts")]
        public async Task<IActionResult> NotifyExpiringContracts()
        {
            try
            {
               
                await _rentalContractService.NotifyExpiringContractsAsync();
                return Ok("Email notifications for expiring contracts sent successfully.");
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }      

    }
}
