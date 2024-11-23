using RentalCar_System.Models.Entity;

namespace RentalCar_System.Data.PaymentRepository
{
    public interface IPaymentRepository
    {
        Task<decimal> GetTotalAmountAsync();
        Task<int> GetPaymentCountByStatusAsync(string status);
        Task<IEnumerable<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
