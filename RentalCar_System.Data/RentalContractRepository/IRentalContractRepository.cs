using RentalCar_System.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalCar_System.Data.RentalContractRepository
{
    public interface IRentalContractRepository
    {
        Task<IEnumerable<RentalContract>> GetAllContractsByUserIdAsync(Guid userId);
        Task<RentalContract> GetRentalContractByIdAsync(Guid contractId);
    }
}
