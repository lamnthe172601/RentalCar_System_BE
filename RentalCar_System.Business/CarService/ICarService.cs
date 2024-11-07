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
    }
}
