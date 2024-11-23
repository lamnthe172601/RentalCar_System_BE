using RentalCar_System.Data.PaymentRepository;
using RentalCar_System.Models.Entity;

namespace RentalCar_System.Business.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _paymentRepository.GetTotalAmountAsync();
        }

        public async Task<int> GetPaymentCountByStatusAsync(string status)
        {
            return await _paymentRepository.GetPaymentCountByStatusAsync(status);
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _paymentRepository.GetPaymentsByDateRangeAsync(startDate, endDate);
        }
    }
}
