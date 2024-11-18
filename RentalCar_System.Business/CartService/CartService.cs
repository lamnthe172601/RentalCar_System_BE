using RentalCar_System.Data.CartRepository;
using RentalCar_System.Models.DtoViewModel;
using RentalCar_System.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalCar_System.Business.CartService
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;

        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<IEnumerable<CartItemDto>> GetCartItemsAsync(Guid userId)
        {
            var cartItems = await _cartRepository.GetCartItemsByUserIdAsync(userId);

            return cartItems.Select(c => new CartItemDto
            {
                CartId = c.CartId,
                CarId = (Guid)c.CarId,
                CarName = c.Car.Name,
                CarModel = c.Car.Model,
                Price = c.Car.Price,
                DateAdded = c.DateAdded ?? DateTime.UtcNow,
                CarImages = c.Car.Images.Select(i => i.Image1).ToList()
            });
        }

        public async Task<bool> AddToCartAsync(Guid userId, Guid carId)
        {
            var existingItems = await _cartRepository.GetCartItemsByUserIdAsync(userId);

            // Check if the car is already in the cart
            if (existingItems.Any(c => c.CarId == carId))
                return false;

            var cartItem = new Cart
            {
                UserId = userId,
                CarId = carId,
                DateAdded = DateTime.UtcNow
            };

            await _cartRepository.AddCartItemAsync(cartItem);
            await _cartRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFromCartAsync(Guid userId, Guid cartId)
        {
            var cartItem = await _cartRepository.GetCartItemByIdAndUserIdAsync(cartId, userId);

            if (cartItem == null) return false;

            await _cartRepository.RemoveCartItemAsync(cartItem);
            await _cartRepository.SaveChangesAsync();
            return true;
        }
    }
}
