using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalCar_System.Models.DtoViewModel
{
    public class CarUpdateDto
    {
        public Guid CarId { get; set; } // Cần CarId để biết đối tượng nào sẽ được cập nhật
        public string Name { get; set; }
        public string LicensePlate { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public int? Seats { get; set; }
        public int? Year { get; set; }
        public string MadeIn { get; set; }
        public decimal? Mileage { get; set; }
        public string Status { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }

        // Thuộc tính để nhận file ảnh mới (nếu cần thay đổi ảnh)
        public IFormFile? Image { get; set; }
    }
}
