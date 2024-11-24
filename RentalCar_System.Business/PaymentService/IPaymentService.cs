using RentalCar_System.Models.Entity;

namespace RentalCar_System.Business.PaymentService
{
    public interface IPaymentService
    {
        Task<decimal> GetTotalRevenueAsync();
        Task<int> GetPaymentCountByStatusAsync(string status);
        Task<IEnumerable<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
