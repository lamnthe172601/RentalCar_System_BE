using Microsoft.AspNetCore.Mvc;
using RentalCar_System.Business.CarService;
using RentalCar_System.Models.Entity;

namespace RentalCar_System.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarController : ControllerBase
    {
        private readonly CarService _carService;

        public CarController(CarService carService)
        {
            _carService = carService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cars = await _carService.GetAllCarsAsync();
            return Ok(cars);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var car = await _carService.GetCarByIdAsync(id);
            if (car == null)
                return NotFound();
            return Ok(car);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Car car)
        {
            await _carService.AddCarAsync(car);
            return CreatedAtAction(nameof(GetById), new { id = car.CarId }, car);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, Car car)
        {
            if (id != car.CarId)
                return BadRequest();
            await _carService.UpdateCarAsync(car);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _carService.DeleteCarAsync(id);
            return NoContent();
        }
    }
}
