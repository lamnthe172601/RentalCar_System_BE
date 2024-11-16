using Microsoft.EntityFrameworkCore;
using RentalCar_System.Models.DtoViewModel;
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
        public async Task<IEnumerable<CarRented>> GetAllContractsByUserIdAsync(Guid userId)
        {
            var rentalContracts = await _context.RentalContracts
                .Include(rc => rc.Car)
                .ThenInclude(c => c.Images).
                Where(rc => rc.UserId == userId && rc.Status.Trim().ToLower() == "pending")
                .ToListAsync();

            var carRentedDTOs = rentalContracts.Select(rc => new CarRented
            {   
                CarId = rc.Car.CarId,
                ContractId = rc.ContractId,
                Name = rc.Car.Name,
                LicensePlate = rc.Car.LicensePlate,
                Brand = rc.Car.Brand,
                Model = rc.Car.Model,
                Color = rc.Car.Color,
                Seats = (int)rc.Car.Seats,
                Year = (int)rc.Car.Year,
                Price = rc.Car.Price,
                RentalDate = rc.RentalDate.ToString("yyyy-MM-dd"),
                ReturnDate = rc.ReturnDate?.ToString("yyyy-MM-dd"),
                RentalTime = rc.RentalDate.ToString("HH:mm"),
                ReturnTime = rc.ReturnDate?.ToString("HH:mm"),
                ImageUrls = rc.Car.Images.Select(img => img.Image1).ToList() 
            }).ToList();

            return carRentedDTOs;
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
        public async Task<List<RentalContract>> GetAllAsync()
        {
            try
            {
                
                return await _context.RentalContracts.ToListAsync();
            }
            catch (Exception)
            {
                
                throw new Exception("An error occurred while retrieving rental contracts.");
            }
        }


        public async Task<User> GetUserByContractIdAsync(Guid contractId)
        {
            try
            {
                var user = await _context.RentalContracts
                    .Where(rc => rc.ContractId == contractId)
                    .Select(rc => rc.User)
                    .FirstOrDefaultAsync();

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving the user for the contract: {ex.Message}", ex);
            }
        }

    }
}
