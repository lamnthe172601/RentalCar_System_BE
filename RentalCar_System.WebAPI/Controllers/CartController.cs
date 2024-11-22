using Microsoft.AspNetCore.Mvc;
using RentalCar_System.Business.CartService;
using RentalCar_System.Models.DtoViewModel;
using System;
using System.Threading.Tasks;

namespace RentalCar_System.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // GET: api/cart/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<CartItemDto>>> GetCartItems(Guid userId)
        {
            var cartItems = await _cartService.GetCartItemsAsync(userId);

            if (cartItems == null)
                return NotFound("No items found in the cart.");

            return Ok(cartItems);
        }

        // POST: api/cart/add
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            await _cartService.AddToCartAsync(request.UserId, request.CarId);

            return Ok(new { Message = "Car added to the cart successfully." });
        }



        // DELETE: api/cart/remove/{cartId}/{userId}
        [HttpDelete("remove/{cartId}/{userId}")]
        public async Task<ActionResult<bool>> RemoveFromCart(Guid cartId, Guid userId)
        {
            var result = await _cartService.RemoveFromCartAsync(userId, cartId);

            if (result)
            {               
                return Ok(new { message = "Car removed from the cart successfully." });
            }
           
            return BadRequest(new { message = "Unable to remove the car from the cart." });
        }
        [HttpDelete("RemoveFromCart/{contractId}")]
        public async Task<IActionResult> RemoveFromCartByContractIdAsync(Guid contractId)
        {
            if (contractId == Guid.Empty)
            {
                return BadRequest("Invalid contract ID.");
            }

            try
            {
                await _cartService.RemoveFromCartByContractIdAsync(contractId);
                return Ok(new { message = "Contract removed from the cart successfully." });
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Contract not found in the cart.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error.");
            }
        }

    }
}
