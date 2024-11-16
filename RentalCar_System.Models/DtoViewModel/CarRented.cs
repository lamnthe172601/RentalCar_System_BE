using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalCar_System.Models.DtoViewModel
{
    public class CarRented
    {
        public Guid CarId { get; set; }
        public Guid ContractId { get; set; }
        public string Name { get; set; }
        public string LicensePlate { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public int Seats { get; set; }
        public int Year { get; set; }
        public decimal Price { get; set; }
        public List<string> ImageUrls { get; set; }
        public string RentalDate { get; set; }
        public string ReturnDate { get; set; }
        public string RentalTime { get; set; }
        public string? ReturnTime { get; set; }
    }
}
