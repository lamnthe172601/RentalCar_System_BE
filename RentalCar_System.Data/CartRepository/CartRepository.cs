using Microsoft.EntityFrameworkCore;
using RentalCar_System.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalCar_System.Data.CartRepository
{
    public class CartRepository : ICartRepository
    {
        private readonly RentalCarDBContext _context;
        public CartRepository(RentalCarDBContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Cart>> GetCartItemsByUserIdAsync(Guid userId)
        {
            return await _context.Carts
                .Where(c => c.UserId == userId)
                .Include(c => c.Car)
                .ThenInclude(c => c.Images)
                .ToListAsync();
        }

        public async Task<Cart> GetCartItemByIdAndUserIdAsync(Guid cartId, Guid userId)
        {
            return await _context.Carts
                .Where(c => c.CartId == cartId && c.UserId == userId)
                .Include(c => c.Car)
                .FirstOrDefaultAsync();
        }

        public async Task AddCartItemAsync(Cart cartItem)
        {
            await _context.Carts.AddAsync(cartItem);
        }

        public async Task RemoveCartItemAsync(Cart cartItem)
        {
            _context.Carts.Remove(cartItem);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
