using RentalCar_System.Data;
using RentalCar_System.Models.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RentalCar_System.Business.SearchService
{
    public class SearchService : ISearchService
    {
        private readonly IBaseRepository<Car> _carRepository;

        public SearchService(IBaseRepository<Car> carRepository)
        {
            _carRepository = carRepository;
        }

        public async Task<IEnumerable<Car>> SearchCars(string name)
        {
            return await _carRepository.GetAllAsync(c =>
                (string.IsNullOrEmpty(name) || c.Name.Contains(name))
            );
        }
    }
}
