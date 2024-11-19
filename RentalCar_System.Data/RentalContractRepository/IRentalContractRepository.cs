using RentalCar_System.Models.DtoViewModel;
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

        Task<IEnumerable<CarRented>> GetAllContractsByUserIdAsync(Guid userId);
        Task<RentalContract> GetRentalContractByIdAsync(Guid contractId);

        Task AddContractAsync(RentalContractDto rentalContractDto);

        Task<RentalContract> UpdateContractAsync(RentalContract rentalContract);
        Task<List<RentalContract>> GetAllAsync();
        Task<User> GetUserByContractIdAsync(Guid contractId);
        Task UpdateContractStatusAsync(Guid contractId, string status);
    }
}
