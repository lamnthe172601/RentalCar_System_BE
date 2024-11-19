using RentalCar_System.Models.DtoViewModel;
using RentalCar_System.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalCar_System.Business.CarService
{
    public interface ICarService
    {
        Task<IEnumerable<Car>> GetAllAvailableCarsAsync();
        Task<Car> GetCarByIdAsync(Guid carId);
        Task<IEnumerable<Car>> GetAllCarsAsync();
        Task<CarDto> GetCarDTOByIdAsync(Guid id); 
        Task AddCarAsync(Car car);

        Task UpdateCarAsync(Car car);

        Task DeleteCarAsync(Guid id);
        Task<Car> GetCarByLicensePlateAsync(string licensePlate);
        Task UpdateStatusCar(Guid carId, string status);
    }
}
