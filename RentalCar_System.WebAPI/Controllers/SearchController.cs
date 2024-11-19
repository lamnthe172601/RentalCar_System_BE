using Microsoft.AspNetCore.Mvc;
using RentalCar_System.Business.SearchService;
using RentalCar_System.Models.DtoViewModel;
using RentalCar_System.Models.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RentalCar_System.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet("cars")]
        public async Task<ActionResult<IEnumerable<Car>>> SearchCars([FromQuery] SearchCar searchCar)
        {
            var cars = await _searchService.SearchCars(searchCar.Name);
            if (cars == null || !cars.Any())
            {
                return NotFound();
            }
            return Ok(cars);
        }
    }
}
