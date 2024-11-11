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
        Task<IEnumerable<Car>> GetAllAsync();
        Task<Car> GetByIdAsync(Guid id);
        Task AddAsync(Car car);
        Task UpdateAsync(Car car);
        Task DeleteAsync(Guid id);
    }
}
