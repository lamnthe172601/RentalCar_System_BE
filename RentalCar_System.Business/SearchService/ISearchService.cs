using RentalCar_System.Models.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RentalCar_System.Business.SearchService
{
    public interface ISearchService
    {
        Task<IEnumerable<Car>> SearchCars(string name);
    }
}
