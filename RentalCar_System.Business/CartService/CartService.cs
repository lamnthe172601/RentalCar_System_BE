using RentalCar_System.Business.QueueService;
using RentalCar_System.Data.CarRepository;
using RentalCar_System.Data.CartRepository;
using RentalCar_System.Models.DtoViewModel;
using RentalCar_System.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace RentalCar_System.Business.CartService
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IQueueService _queueService;
        private readonly ICarRepository _carRepository;

        public CartService(IQueueService queueService , ICartRepository cartRepository , ICarRepository carRepository)
        {
            _cartRepository = cartRepository;
            _queueService = queueService;
            _carRepository = carRepository;
        }

        public async Task<IEnumerable<CartItemDto>> GetCartItemsAsync(Guid userId)
        {
            var cartItems = await _cartRepository.GetCartItemsByUserIdAsync(userId);

            return cartItems.Select(c => new CartItemDto
            {
                CartId = c.CartId,
                CarId = (Guid)c.CarId,
                CarName = c.Car.Name,
                CarStatus = c.Car.Status,
                Price = c.Car.Price,
                DateAdded = c.DateAdded ?? DateTime.UtcNow,
                CarImages = c.Car.Images.Select(i => Convert.ToBase64String(i.Photo)).ToList()
            });
        }

        public async Task AddToCartAsync(Guid userId, Guid carId)
        {
            var request = new AddToCartRequest { UserId = userId, CarId = carId };
            _queueService.Enqueue(request); 
            await ProcessQueueAsync();     
        }

        private async Task ProcessQueueAsync()
        {
            if (_queueService.IsQueueEmpty()) return;

            var request = _queueService.Dequeue();

            
            var existingItems = await _cartRepository.GetCartItemsByUserIdAsync(request.UserId);
            if (existingItems.Any(c => c.CarId == request.CarId))
            {
                
                return;
            }

            var cartItem = new Cart
            {
                UserId = request.UserId,
                CarId = request.CarId,
                DateAdded = DateTime.UtcNow
            };

          
            await _cartRepository.AddCartItemAsync(cartItem);
            await _cartRepository.SaveChangesAsync();
        }

        public async Task<bool> RemoveFromCartAsync(Guid userId, Guid cartId)
        {
            var cartItem = await _cartRepository.GetCartItemByIdAndUserIdAsync(cartId, userId);

            if (cartItem == null) return false;

            await _cartRepository.RemoveCartItemAsync(cartItem);
            await _cartRepository.SaveChangesAsync();
            return true;
        }
        public async Task<decimal> GetTotalPriceAsync(Guid userId)
        {
            var cartItems = await _cartRepository.GetCartItemsByUserIdAsync(userId);

            
            return cartItems.Sum(c => c.Car.Price);
        }
        public async Task RemoveFromCartByContractIdAsync(Guid contractId)
        {
            await _cartRepository.RemoveFromCartByContractIdAsync(contractId);
        }
    }
}
