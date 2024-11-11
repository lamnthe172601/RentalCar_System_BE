using Microsoft.EntityFrameworkCore;
using RentalCar_System.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RentalCar_System.Models.DtoViewModel;

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

        public async Task<Car> GetCarByIdAsync(Guid id)
        {
            return await _context.Cars
                .Include(c => c.Images) // Bao gồm danh sách Images
                .FirstOrDefaultAsync(c => c.CarId == id);
        }
        public async Task<IEnumerable<Car>> GetAllAsync()
        {
            return await _context.Cars.Include(c => c.Images).ToListAsync();
        }

        public async Task<Car> GetByIdAsync(Guid id)
        {
            return await _context.Cars.Include(c => c.Images).FirstOrDefaultAsync(c => c.CarId == id);
        }

        public async Task AddCarAsync(Car car)
        {
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateCarAsync(Car car)
        {
            _context.Cars.Update(car);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCarAsync(Guid id)
        {
            var car = await _context.Cars
                .Include(c => c.Images) // Bao gồm các ảnh liên kết
                .FirstOrDefaultAsync(c => c.CarId == id);

            if (car != null)
            {
                _context.Cars.Remove(car); // Xóa Car
                await _context.SaveChangesAsync();
            }
        }
        public async Task<Car> GetCarByLicensePlateAsync(string licensePlate)
        {
            return await _context.Cars
                .FirstOrDefaultAsync(c => c.LicensePlate == licensePlate); // Kiểm tra Car có LicensePlate trùng
        }
     

        public async Task<CarDto> GetCarDtoByIdAsync(Guid id)
        {
            var car = await _context.Cars.Include(c => c.Images).FirstOrDefaultAsync(c => c.CarId == id);
            if (car == null) return null;

            return new CarDto
            {
                CarId = car.CarId,
                Name = car.Name,
                LicensePlate = car.LicensePlate,
                Brand = car.Brand,
                Model = car.Model,
                Color = car.Color,
                Seats = car.Seats,
                Year = car.Year,
                MadeIn = car.MadeIn,
                Mileage = car.Mileage,
                Status = car.Status,
                Price = car.Price,
                Description = car.Description,
                Images = car.Images.Select(img => $"/images/{img.Image1}").ToList()
            };
        }
    }
}