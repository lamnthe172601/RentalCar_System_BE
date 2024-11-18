using RentalCar_System.Models.DtoViewModel;
using RentalCar_System.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalCar_System.Data.CartRepository
{
    public interface ICartRepository
    {
        Task<IEnumerable<Cart>> GetCartItemsByUserIdAsync(Guid userId);
        Task<Cart> GetCartItemByIdAndUserIdAsync(Guid cartId, Guid userId);
        Task AddCartItemAsync(Cart cartItem);
        Task RemoveCartItemAsync(Cart cartItem);
        Task SaveChangesAsync();
    }
}
