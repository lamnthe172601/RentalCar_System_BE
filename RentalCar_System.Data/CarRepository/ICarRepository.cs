using RentalCar_System.Models.DtoViewModel;
using RentalCar_System.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalCar_System.Data.CarRepository
{
    public interface ICarRepository
    {
        Task<IEnumerable<Car>> GetAllAvailableCarsAsync();
        Task<Car> GetCarByIdAsync(Guid carId);
        Task<IEnumerable<Car>> GetAllAsync();
        Task<Car> GetByIdAsync(Guid id);
        Task AddCarAsync(Car car);
        Task<CarDto> GetCarDtoByIdAsync(Guid id);
        Task UpdateCarAsync(Car car);

        Task DeleteCarAsync(Guid id);
        
    }
}
