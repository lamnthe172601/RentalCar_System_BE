using RentalCar_System.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RentalCar_System.Data.CarRepository
{
    public class CarRepository : ICarRepository
    {
        private readonly RentalCarDBContext _context;

        public CarRepository(RentalCarDBContext context)
        {
            _context = context;
        }

        public async Task<Car> GetByIdAsync(Guid carId) => await _context.Cars.FindAsync(carId);

        public async Task<IEnumerable<Car>> GetAllAsync() => await _context.Cars.ToListAsync();

        public async Task AddAsync(Car car)
        {
            await _context.Cars.AddAsync(car);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Car car)
        {
            _context.Cars.Update(car);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid carId)
        {
            var car = await _context.Cars.FindAsync(carId);
            if (car != null)
            {
                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();
            }
        }
    }
}
