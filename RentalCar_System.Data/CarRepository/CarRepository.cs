using Microsoft.EntityFrameworkCore;
using RentalCar_System.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalCar_System.Data.CarRepository
{
    public class CarRepository : ICarRepository
    {
        private readonly RentalCarDBContext _context;

        public CarRepository(RentalCarDBContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Car>> GetAllAvailableCarsAsync()
        {
            return await _context.Cars
           .Where(car => car.Status == "Available")
           .ToListAsync();
        }

        public async Task<Car> GetCarByIdAsync(Guid carId)
        {
            return await _context.Cars
           .FirstOrDefaultAsync(car => car.CarId == carId);
        }
    }
}
