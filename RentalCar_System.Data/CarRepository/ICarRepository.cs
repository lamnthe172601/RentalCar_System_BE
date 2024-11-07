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
    }
}
