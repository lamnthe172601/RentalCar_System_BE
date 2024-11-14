using RentalCar_System.Business.BaseService;
using Microsoft.AspNetCore.Mvc;
using RentalCar_System.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RentalCar_System.Models.DtoViewModel;

namespace RentalCar_System.Business.RentalCarService
{
    public interface IRentalContractService
    {
         Task<IEnumerable<CarRented>> GetAllContractsByUserIdAsync(Guid userId);
        Task<RentalContract> GetRentalContractByIdAsync(Guid contractId);

        Task<RentalContract> SendRentRequestAsync(Guid userId, Guid carId, DateTime rentalDate, DateTime returnDate);

        Task<bool> CancelRentalContractAsync(Guid contractId);

        Task<bool> UpdateFeedbackAndRatingAsync(Guid contractId, string feedback, int rating);
        Task NotifyExpiringContractsAsync();
       
      
    }
}
