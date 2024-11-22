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
        public async Task RemoveFromCartByContractIdAsync(Guid contractId)
        {
            // Lấy hợp đồng với contractId
            var contractItem = await _context.RentalContracts
                .FirstOrDefaultAsync(rc => rc.ContractId == contractId);

            // Nếu không tìm thấy hợp đồng, ném ngoại lệ
            if (contractItem == null)
            {
                throw new KeyNotFoundException("Contract not found.");
            }

            // Lấy các mục trong giỏ hàng tương ứng với CarId của hợp đồng
            var cartItems = await _context.Carts
                .Where(c => c.CarId == contractItem.CarId)
                .ToListAsync();

            // Xóa các mục trong giỏ hàng
            _context.Carts.RemoveRange(cartItems);

            // Lưu các thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();
        }
    }
}
