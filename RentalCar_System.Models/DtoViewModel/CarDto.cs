using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalCar_System.Models.DtoViewModel
{
    public class CarDto
    {
        public Guid CarId { get; set; }
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
        public List<string> Images { get; set; }
        public IFormFile Image { get; set; }
    }
}
