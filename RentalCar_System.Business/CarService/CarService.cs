using RentalCar_System.Data.CarRepository;
using RentalCar_System.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RentalCar_System.Data.CarRepository;
using RentalCar_System.Models.Entity;
using RentalCar_System.Models.DtoViewModel;

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
        public async Task<IEnumerable<Car>> GetAllCarsAsync()
        {
            return await _carRepository.GetAllAsync();
        }

        public async Task<CarDto> GetCarDTOByIdAsync(Guid id)
        {
            return await _carRepository.GetCarDtoByIdAsync(id); // Trả về DTO
        }
        

        public async Task AddCarAsync(Car car)
        {
            await _carRepository.AddCarAsync(car);
        }
        public async Task UpdateCarAsync(Car car)
        {
            await _carRepository.UpdateCarAsync(car);
        }
        public async Task DeleteCarAsync(Guid id)
        {
            await _carRepository.DeleteCarAsync(id);
        }
        public async Task<Car> GetCarByLicensePlateAsync(string licensePlate)
        {
            return await _carRepository.GetCarByLicensePlateAsync(licensePlate);
        }
    }
}

