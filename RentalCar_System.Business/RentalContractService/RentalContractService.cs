using RentalCar_System.Business.BaseService;
using RentalCar_System.Data;
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
        private readonly IBaseRepository<RentalContract> _baseRepository;

        public RentalContractService(IBaseRepository<RentalContract> baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public async Task<IEnumerable<RentalContract>> GetAllContractsByUserIdAsync(Guid userId)
        {
            return await _baseRepository.GetAllAsync(rc => rc.UserId == userId);
        }

        public async Task<RentalContract> GetByIdAsync(Guid id)
        {
            return await _baseRepository.GetByIdAsync(id);
        }       

        

        public async Task<RentalContract> GetRentalContractByIdAsync(Guid contractId)
        {
            return await _baseRepository.GetByIdAsync(contractId);
        }

       
    }
}
