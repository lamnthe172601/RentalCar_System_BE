using RentalCar_System.Models.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalCar_System.Models.DtoViewModel
{
    public class ResetPasswordRequest
    {
        public string Token { get; set; }
        [Required(ErrorMessage = "Password is required")]

        [PasswordValidator]
        public string NewPassword { get; set; }
    }
}
