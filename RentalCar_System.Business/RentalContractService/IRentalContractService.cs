using RentalCar_System.Business.BaseService;
using Microsoft.AspNetCore.Mvc;
using RentalCar_System.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalCar_System.Business.RentalCarService
{
    public interface IRentalContractService
    {
         Task<IEnumerable<RentalContract>> GetAllContractsByUserIdAsync(Guid userId);
        Task<RentalContract> GetRentalContractByIdAsync(Guid contractId);

        Task<RentalContract> SendRentRequestAsync(Guid userId, Guid carId, DateTime rentalDate, DateTime returnDate);
    }
}
