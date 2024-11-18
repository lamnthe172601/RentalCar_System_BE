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
        public async Task<ActionResult<bool>> AddToCart([FromBody] AddToCartRequest request)
        {
            var result = await _cartService.AddToCartAsync(request.UserId, request.CarId);

            if (result)
                return Ok("Car added to the cart successfully.");

            return BadRequest("The car is already in the cart.");
        }

        // DELETE: api/cart/remove/{cartId}/{userId}
        [HttpDelete("remove/{cartId}/{userId}")]
        public async Task<ActionResult<bool>> RemoveFromCart(Guid cartId, Guid userId)
        {
            var result = await _cartService.RemoveFromCartAsync(userId, cartId);

            if (result)
                return Ok("Car removed from the cart successfully.");

            return BadRequest("Unable to remove the car from the cart.");
        }
    }
}
