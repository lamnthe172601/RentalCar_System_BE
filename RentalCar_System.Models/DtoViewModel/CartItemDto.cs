using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalCar_System.Models.DtoViewModel
{
    public class CartItemDto
    {
        public Guid CartId { get; set; }    
        public Guid CarId { get; set; }     
        public string CarName { get; set; } 

        public string CarModel { get; set; }
        public decimal Price { get; set; }  
        public DateTime DateAdded { get; set; }

        public List<byte[]> CarImages { get; set; } = new List<byte[]>();
    }
}
