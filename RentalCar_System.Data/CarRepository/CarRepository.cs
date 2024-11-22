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
            var existingCar = await _context.Cars.Include(c => c.Images)
                                                 .FirstOrDefaultAsync(c => c.CarId == car.CarId);

            if (existingCar != null)
            {
                // Cập nhật thông tin Car
                existingCar.Name = car.Name ?? existingCar.Name;
                existingCar.LicensePlate = car.LicensePlate ?? existingCar.LicensePlate;
                existingCar.Brand = car.Brand ?? existingCar.Brand;
                existingCar.Model = car.Model ?? existingCar.Model;
                existingCar.Color = car.Color ?? existingCar.Color;
                existingCar.Seats = car.Seats ?? existingCar.Seats;
                existingCar.Year = car.Year ?? existingCar.Year;
                existingCar.MadeIn = car.MadeIn ?? existingCar.MadeIn;
                existingCar.Mileage = car.Mileage ?? existingCar.Mileage;
                existingCar.Status = car.Status ?? existingCar.Status;
                existingCar.Price = car.Price;
                existingCar.Description = car.Description ?? existingCar.Description;

                // Nếu có ảnh mới, xóa ảnh cũ và thêm ảnh mới
                if (car.Images != null && car.Images.Count > 0)
                {
                    _context.Images.RemoveRange(existingCar.Images); // Xóa ảnh cũ
                    await _context.Images.AddRangeAsync(car.Images); // Thêm ảnh mới
                }

                await _context.SaveChangesAsync();
            }
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
                Images = car.Images.Select(img => Convert.ToBase64String(img.Photo)).ToList()
            };
        }
        public async Task UpdateStatusCar(Guid contractId, string status)
        {
            
            var rentalContract = await _context.RentalContracts.FirstOrDefaultAsync(rc => rc.ContractId == contractId);
            if (rentalContract != null)
            {
               
                var carId = rentalContract.CarId;

                
                var car = await _context.Cars.FirstOrDefaultAsync(c => c.CarId == carId);
                if (car != null)
                {
                    
                    car.Status = status;
                    _context.Cars.Update(car);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}