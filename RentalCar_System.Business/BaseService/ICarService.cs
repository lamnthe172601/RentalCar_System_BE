using RentalCar_System.Models.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;
using RentalCar_System.Business.BaseService; // Add this using directive

namespace RentalCar_System.Business.CarService
{
    public interface ICarService : IBaseService<Car>
    {
        Task<IEnumerable<Car>> SearchCarsByName(string name);
    }
}
