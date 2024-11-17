using RentalCar_System.Models.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalCar_System.Models.DtoViewModel
{
    public class VerifyEmailRequest
    {
        public string Token { get; set; }
        [Required(ErrorMessage = "Email is required")]    
        public string Email  { get; set; }
    }
}
