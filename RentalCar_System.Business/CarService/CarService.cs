using RentalCar_System.Business.BaseService;
using RentalCar_System.Data;
using RentalCar_System.Models.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RentalCar_System.Business.CarService
{
    public class CarService : BaseService<Car>, ICarService
    {
        public CarService(IBaseRepository<Car> baseRepository) : base(baseRepository)
        {
        }

        public async Task<IEnumerable<Car>> SearchCarsByName(string name)
        {
            return await _baseRepository.GetAllAsync(c => c.Name.Contains(name));
        }
    }
}
