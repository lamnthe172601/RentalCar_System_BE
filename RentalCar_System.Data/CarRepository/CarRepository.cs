using Microsoft.EntityFrameworkCore;
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
        public async Task<IEnumerable<Car>> GetAllAsync()
        {
            return await _context.Cars.ToListAsync();
        }

        public async Task<Car> GetByIdAsync(Guid id)
        {
            return await _context.Cars.FindAsync(id);
        }

        public async Task AddAsync(Car car)
        {
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Car car)
        {
            if (car == null) throw new ArgumentNullException(nameof(car));
            _context.Entry(car).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null) throw new KeyNotFoundException($"Car with ID {id} not found.");
            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
        }
    }
}