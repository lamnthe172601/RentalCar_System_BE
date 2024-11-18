using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalCar_System.Business.CarService;
using RentalCar_System.Models.DtoViewModel;
using RentalCar_System.Models.Entity;
using static System.Net.Mime.MediaTypeNames;

namespace RentalCar_System.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarController : ControllerBase
    {
        private readonly ICarService _carService;
        private const string ImagesFolder = "Images";
        public CarController(ICarService carService)
        {
            _carService = carService;
        }

        [HttpGet("all-car")]
        public async Task<IActionResult> GetAllCars()
        {
            var cars = await _carService.GetAllCarsAsync();
            var carDtos = cars.Select(car => new CarDto
            {
                CarId = car.CarId,
                Name = car.Name,
                LicensePlate = car.LicensePlate,
                Brand = car.Brand,
                Model = car.Model,
                Color = car.Color,
                Seats = car.Seats,
                Year = car.Year,
                MadeIn = car.MadeIn,
                Mileage = car.Mileage,
                Status = car.Status,
                Price = car.Price,
                Description = car.Description,
                Images = car.Images.Select(img => Convert.ToBase64String(img.Photo)).ToList() // Chỉ trả về đường dẫn ảnh
            }).ToList();

            return Ok(carDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CarDto>> GetCarById(Guid id)
        {
            var car = await _carService.GetCarByIdAsync(id);
            if (car == null) return NotFound();

            // Chuyển đổi entity Car sang CarDto
            var carDto = new CarDto
            {
                CarId = car.CarId,
                Name = car.Name,
                LicensePlate = car.LicensePlate,
                Brand = car.Brand,
                Model = car.Model,
                Color = car.Color,
                Seats = car.Seats,
                Year = car.Year,
                MadeIn = car.MadeIn,
                Mileage = car.Mileage,
                Status = car.Status,
                Price = car.Price,
                Description = car.Description,
                Images = car.Images.Select(img => Convert.ToBase64String(img.Photo)).ToList() 
            };

            return Ok(carDto);
        }

        // POST: api/Car

        [HttpPost("add-car")]
        
        public async Task<IActionResult> PostCar([FromForm] CarCreateDto carCreateDto)
        {
            if (carCreateDto == null) return BadRequest("Invalid car data");

            byte[] imageBytes = null;

            // Lưu ảnh dưới dạng byte array
            if (carCreateDto.Image != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await carCreateDto.Image.CopyToAsync(memoryStream);
                    imageBytes = memoryStream.ToArray();
                }
            }

            // Ánh xạ từ DTO sang entity Car
            var carId = Guid.NewGuid();
            var car = new Car
            {
                CarId = carId, // Đảm bảo CarId được khởi tạo
                Name = carCreateDto.Name,
                LicensePlate = carCreateDto.LicensePlate,
                Brand = carCreateDto.Brand,
                Model = carCreateDto.Model,
                Color = carCreateDto.Color,
                Seats = carCreateDto.Seats,
                Year = carCreateDto.Year,
                MadeIn = carCreateDto.MadeIn,
                Mileage = carCreateDto.Mileage,
                Status = carCreateDto.Status,
                Price = carCreateDto.Price,
                Description = carCreateDto.Description,
                Images = imageBytes != null ? new List<Models.Entity.Image>
        {
            new Models.Entity.Image
            {
                ImgId = Guid.NewGuid(),
                CarId = carId,
                Photo = imageBytes
            }
        } : new List<Models.Entity.Image>()
            };

            await _carService.AddCarAsync(car);

            var createdCarDto = new CarDto
            {
                CarId = car.CarId,
                Name = car.Name,
                LicensePlate = car.LicensePlate,
                Brand = car.Brand,
                Model = car.Model,
                Color = car.Color,
                Seats = car.Seats,
                Year = car.Year,
                MadeIn = car.MadeIn,
                Mileage = car.Mileage,
                Status = car.Status,
                Price = car.Price,
                Description = car.Description,
                Images = car.Images.Select(img => Convert.ToBase64String(img.Photo)).ToList()
            };

            return CreatedAtAction(nameof(GetCarById), new { id = createdCarDto.CarId }, createdCarDto);
        }


        [HttpPut("edit-car")]
        public async Task<IActionResult> UpdateCar(Guid id, [FromForm] CarUpdateDto carUpdateDto)
        {
            if (id == Guid.Empty || carUpdateDto == null) return BadRequest("Invalid data.");

            var existingCar = await _carService.GetCarByIdAsync(id);
            if (existingCar == null) return NotFound("Car not found.");

            byte[] imageBytes = null;

            // Upload ảnh mới (nếu có)
            if (carUpdateDto.Image != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await carUpdateDto.Image.CopyToAsync(memoryStream);
                    imageBytes = memoryStream.ToArray();
                }
            }

            // Cập nhật thông tin Car
            existingCar.Name = carUpdateDto.Name ?? existingCar.Name;
            existingCar.LicensePlate = carUpdateDto.LicensePlate ?? existingCar.LicensePlate;
            existingCar.Brand = carUpdateDto.Brand ?? existingCar.Brand;
            existingCar.Model = carUpdateDto.Model ?? existingCar.Model;
            existingCar.Color = carUpdateDto.Color ?? existingCar.Color;
            existingCar.Seats = carUpdateDto.Seats ?? existingCar.Seats;
            existingCar.Year = carUpdateDto.Year ?? existingCar.Year;
            existingCar.MadeIn = carUpdateDto.MadeIn ?? existingCar.MadeIn;
            existingCar.Mileage = carUpdateDto.Mileage ?? existingCar.Mileage;
            existingCar.Status = carUpdateDto.Status ?? existingCar.Status;
            existingCar.Price = carUpdateDto.Price;
            existingCar.Description = carUpdateDto.Description ?? existingCar.Description;

            if (imageBytes != null)
            {
                // Xóa ảnh cũ và thêm ảnh mới
                existingCar.Images.Clear();
                existingCar.Images.Add(new Models.Entity.Image
                {
                    ImgId = Guid.NewGuid(),
                    CarId = existingCar.CarId,
                    Photo = imageBytes
                });
            }

            await _carService.UpdateCarAsync(existingCar);

            return Ok(new CarDto
            {
                CarId = existingCar.CarId,
                Name = existingCar.Name,
                LicensePlate = existingCar.LicensePlate,
                Brand = existingCar.Brand,
                Model = existingCar.Model,
                Color = existingCar.Color,
                Seats = existingCar.Seats,
                Year = existingCar.Year,
                MadeIn = existingCar.MadeIn,
                Mileage = existingCar.Mileage,
                Status = existingCar.Status,
                Price = existingCar.Price,
                Description = existingCar.Description,
                Images = existingCar.Images.Select(img => Convert.ToBase64String(img.Photo)).ToList()
            });
        }




        [HttpDelete("id")]
        public async Task<IActionResult> DeleteCar(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid Car ID.");

            var existingCar = await _carService.GetCarByIdAsync(id);
            if (existingCar == null)
                return NotFound("Car not found.");

            // Xóa ảnh liên kết
            await _carService.DeleteCarAsync(id);

            return Ok(new CarDto
            {
                CarId = existingCar.CarId,
                Name = existingCar.Name,
                LicensePlate = existingCar.LicensePlate,
                Brand = existingCar.Brand,
                Model = existingCar.Model,
                Color = existingCar.Color,
                Seats = existingCar.Seats,
                Year = existingCar.Year,
                MadeIn = existingCar.MadeIn,
                Mileage = existingCar.Mileage,
                Status = existingCar.Status,
                Price = existingCar.Price,
                Description = existingCar.Description,
                Images = existingCar.Images.Select(img => Convert.ToBase64String(img.Photo)).ToList()
            });
        }
    }

}