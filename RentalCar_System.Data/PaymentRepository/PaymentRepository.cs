using Microsoft.EntityFrameworkCore;
using RentalCar_System.Models.Entity;

namespace RentalCar_System.Data.PaymentRepository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly RentalCarDBContext _context;

        public PaymentRepository(RentalCarDBContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetTotalAmountAsync()
        {
            return await _context.Payments.SumAsync(p => p.Amount);
        }

        public async Task<int> GetPaymentCountByStatusAsync(string status)
        {
            return await _context.Payments.CountAsync(p => p.Status == status);
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Payments
                .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
                .ToListAsync();
        }
    }
}
