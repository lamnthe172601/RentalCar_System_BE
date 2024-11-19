using RentalCar_System.Data.CartRepository;
using RentalCar_System.Models.DtoViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalCar_System.Business.CartService
{
    public interface ICartService
    {
        Task<IEnumerable<CartItemDto>> GetCartItemsAsync(Guid userId);
        Task<decimal> AddToCartAsync(Guid userId, Guid carId);
        Task<bool> RemoveFromCartAsync(Guid userId, Guid cartId);
        Task<decimal> GetTotalPriceAsync(Guid userId);
        Task RemoveFromCartByContractIdAsync(Guid contractId);
    }
}
