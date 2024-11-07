using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RentalCar_System.Data.CarRepository;
using RentalCar_System.Models.Entity;

namespace RentalCar_System.Business.CarService
{
    public class CarService
    {
        private readonly ICarRepository _carRepository;

        public CarService(ICarRepository carRepository)
        {
            _carRepository = carRepository;
        }

        public async Task<IEnumerable<Car>> GetAllCarsAsync() => await _carRepository.GetAllAsync();

        public async Task<Car> GetCarByIdAsync(Guid carId) => await _carRepository.GetByIdAsync(carId);

        public async Task AddCarAsync(Car car) => await _carRepository.AddAsync(car);

        public async Task UpdateCarAsync(Car car) => await _carRepository.UpdateAsync(car);

        public async Task DeleteCarAsync(Guid carId) => await _carRepository.DeleteAsync(carId);
    }
}
