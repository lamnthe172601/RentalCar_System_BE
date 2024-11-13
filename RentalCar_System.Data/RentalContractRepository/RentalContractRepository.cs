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
            return await _context.RentalContracts
                .Where(rc => rc.UserId == userId) .ToListAsync();
        }
        public async Task<RentalContract> GetRentalContractByIdAsync(Guid contractId)
        {
            return await _context.RentalContracts
                .FirstOrDefaultAsync(rc => rc.ContractId == contractId);
        }


        public async Task<RentalContract> AddContractAsync(RentalContract rentalContract)
        {
            await _context.RentalContracts.AddAsync(rentalContract);
            await _context.SaveChangesAsync();
            return rentalContract;
        }

        public async Task<RentalContract> UpdateContractAsync(RentalContract rentalContract)
        {
            _context.RentalContracts.Update(rentalContract); 
            await _context.SaveChangesAsync(); 
            return rentalContract;
        }



    }
}
