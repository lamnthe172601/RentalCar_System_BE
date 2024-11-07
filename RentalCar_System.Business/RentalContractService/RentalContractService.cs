using RentalCar_System.Business.BaseService;
using RentalCar_System.Data;
using RentalCar_System.Data.RentalContractRepository;
using RentalCar_System.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalCar_System.Business.RentalCarService
{
    public class RentalContractService : IRentalContractService
    {
        private readonly IRentalContractRepository _rentalContractRepository;

        public RentalContractService(IRentalContractRepository rentalContractRepository)
        {
            _rentalContractRepository = rentalContractRepository;
        }

        public async Task<IEnumerable<RentalContract>> GetAllContractsByUserIdAsync(Guid userId)
        {
            return await _rentalContractRepository.GetAllContractsByUserIdAsync(userId);
        }

        public async Task<RentalContract> GetRentalContractByIdAsync(Guid contractId)
        {
            return await _rentalContractRepository.GetRentalContractByIdAsync(contractId);
        }


    }
}
