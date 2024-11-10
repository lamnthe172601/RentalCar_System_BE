using Microsoft.AspNetCore.Mvc;
using RentalCar_System.Business.CarService;
using RentalCar_System.Models.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RentalCar_System.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly ICarService _carService;

        public CarController(ICarService carService)
        {
            _carService = carService;
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Car>>> SearchCarsByName(string name)
        {
            var cars = await _carService.SearchCarsByName(name);
            if (cars == null)
            {
                return NotFound();
            }
            return Ok(cars);
        }
    }
}
