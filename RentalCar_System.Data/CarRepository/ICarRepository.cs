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
        Task<Car> GetByIdAsync(Guid carId);
        Task<IEnumerable<Car>> GetAllAsync();
        Task AddAsync(Car car);
        Task UpdateAsync(Car car);
        Task DeleteAsync(Guid carId);
    }
}
