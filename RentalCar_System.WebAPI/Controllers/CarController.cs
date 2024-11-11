using Microsoft.AspNetCore.Mvc;
using RentalCar_System.Business.CarService;
using RentalCar_System.Models.Entity;

namespace RentalCar_System.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarController : ControllerBase
    {
        private readonly ICarService _carService;

        public CarController(ICarService carService)
        {
            _carService = carService;
        }

        // GET: api/Car
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Car>>> GetCars()
        {
            return Ok(await _carService.GetAllAsync());
        }

        [HttpGet("get-image/{fileName}")]
        public IActionResult GetImage(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "images", fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found");
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "image/jpeg"); // Hoặc thay đổi content-type theo loại file
        }

        // GET: api/Car/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Car>> GetCar(Guid id)
        {
            var car = await _carService.GetByIdAsync(id);

            if (car == null)
            {
                return NotFound();
            }

            return Ok(car);
        }

        // POST: api/Car
        [HttpPost]
        public async Task<ActionResult<Car>> PostCar([FromBody] Car car)
        {
            if (car == null) return BadRequest("Car is null");

            await _carService.AddAsync(car);
            return CreatedAtAction(nameof(GetCar), new { id = car.CarId }, car);
        }

        // PUT: api/Car/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCar(Guid id, [FromBody] Car car)
        {
            if (id != car.CarId)
            {
                return BadRequest("Car ID mismatch");
            }

            await _carService.UpdateAsync(car);
            return NoContent();
        }

        // DELETE: api/Car/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(Guid id)
        {
            await _carService.DeleteAsync(id);
            return NoContent();
        }
    }
}
