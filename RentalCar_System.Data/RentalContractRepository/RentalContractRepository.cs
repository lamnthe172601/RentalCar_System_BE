using Microsoft.EntityFrameworkCore;
using RentalCar_System.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalCar_System.Data.RentalContractRepository
{
    public class RentalContractRepository : IRentalContractRepository
    {
        private readonly RentalCarDBContext _context;

        public RentalContractRepository(RentalCarDBContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<RentalContract>> GetAllContractsByUserIdAsync(Guid userId)
        {
            return await _context.RentalContracts.Include(rc => rc.Car)
                .Where(rc => rc.UserId == userId) .ToListAsync();
        }
        public async Task<RentalContract> GetRentalContractByIdAsync(Guid contractId)
        {
            return await _context.RentalContracts.Include(rc => rc.Car)
                .FirstOrDefaultAsync(rc => rc.ContractId == contractId);
        }
    }
}
