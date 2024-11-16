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

        [HttpGet]
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
                Images = car.Images.Select(img => img.Image1).ToList() // Chỉ trả về đường dẫn ảnh
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
                Images = car.Images.Select(img => img.Image1).ToList() 
            };

            return Ok(carDto);
        }

        // POST: api/Car
        [HttpPost]
        public async Task<IActionResult> PostCar([FromForm] CarCreateDto carCreateDto)
        {
            if (carCreateDto == null) return BadRequest("Invalid car data");

            string publicImagePath = null;

            // Lưu ảnh vào thư mục Images
            if (carCreateDto.Image != null)
            {
                var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), ImagesFolder);

                if (!Directory.Exists(imagesPath))
                {
                    Directory.CreateDirectory(imagesPath);
                }

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(carCreateDto.Image.FileName)}";
                var filePath = Path.Combine(imagesPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await carCreateDto.Image.CopyToAsync(stream);
                }

                // Tạo đường dẫn công khai cho ảnh
                publicImagePath = $"/api/cars/images/{fileName}";
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
                Images = new List<Models.Entity.Image>
    {
        new Models.Entity.Image
        {
            ImgId = Guid.NewGuid(), // Tạo ID mới cho ảnh
            CarId = carId,          // Liên kết với CarId
            Image1 = publicImagePath
        }
    }
            };


            await _carService.AddCarAsync(car);

            // Trả về DTO
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
                Images = car.Images.Select(i => i.Image1).ToList()
            };

            return CreatedAtAction(nameof(GetCarById), new { id = createdCarDto.CarId }, createdCarDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCar(Guid id, [FromForm] CarUpdateDto carUpdateDto)
        {
            if (id == Guid.Empty || carUpdateDto == null) return BadRequest("Invalid data.");

            var existingCar = await _carService.GetCarByIdAsync(id);
            if (existingCar == null) return NotFound("Car not found.");

            string publicImagePath = null;

            // Upload ảnh mới (nếu có)
            if (carUpdateDto.Image != null)
            {
                var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), ImagesFolder);

                if (!Directory.Exists(imagesPath))
                {
                    Directory.CreateDirectory(imagesPath);
                }

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(carUpdateDto.Image.FileName)}";
                var filePath = Path.Combine(imagesPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await carUpdateDto.Image.CopyToAsync(stream);
                }

                publicImagePath = $"/api/cars/images/{fileName}";
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

            if (publicImagePath != null)
            {
                // Xóa ảnh cũ và thêm ảnh mới
                existingCar.Images.Clear();
                existingCar.Images.Add(new Models.Entity.Image
                {
                    ImgId = Guid.NewGuid(),
                    CarId = existingCar.CarId,
                    Image1 = publicImagePath
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
                Images = existingCar.Images.Select(i => i.Image1).ToList()
            });
        }



        // DELETE: api/Car/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid Car ID.");

            var existingCar = await _carService.GetCarByIdAsync(id);
            if (existingCar == null)
                return NotFound("Car not found.");

            // Xóa ảnh liên kết
            var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "Images");
            foreach (var image in existingCar.Images)
            {
                var imagePath = Path.Combine(imagesFolder, Path.GetFileName(image.Image1));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

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
                Images = existingCar.Images.Select(img => img.Image1).ToList()
            });
        }

    }

}