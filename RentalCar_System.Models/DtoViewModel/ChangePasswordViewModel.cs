using RentalCar_System.Models.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalCar_System.Models.DtoViewModel
{
    public class ChangePasswordViewModel {
        
        [Required(ErrorMessage = "Password is required")]
        public string OldPassword { get; set; }

        [PasswordValidator]
        public string NewPassword { get; set; } }
}
