using RentalCar_System.Data.CarRepository;
using RentalCar_System.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalCar_System.Business.CarService
{
    public class CarService : ICarService
    {
        private readonly ICarRepository _carRepository;

        public CarService(ICarRepository carRepository)
        {
            _carRepository = carRepository;
        }
        public async Task<IEnumerable<Car>> GetAllAvailableCarsAsync()
        {
            return await _carRepository.GetAllAvailableCarsAsync();
        }

        public async Task<Car> GetCarByIdAsync(Guid carId)
        {
            return await _carRepository.GetCarByIdAsync(carId);
        }
        public async Task<IEnumerable<Car>> GetAllAsync()
        {
            return await _carRepository.GetAllAsync();
        }

        public async Task<Car> GetByIdAsync(Guid id)
        {
            return await _carRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(Car car)
        {
            await _carRepository.AddAsync(car);
        }

        public async Task UpdateAsync(Car car)
        {
            await _carRepository.UpdateAsync(car);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _carRepository.DeleteAsync(id);
        }
    }
}
