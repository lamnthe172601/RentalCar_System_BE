using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RentalCar_System.Models.Validations
{
    public class PasswordValidator : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            { return new ValidationResult("Password is required"); }

            var password = value as string;
            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperCase = new Regex(@"[A-Z]+");
            var hasLowerCase = new Regex(@"[a-z]+");
            var hasMinMaxCharacter = new Regex(@".{8,}");
            var hasSpecialCharacter = new Regex(@"[\W]+");
            if (!hasNumber.IsMatch(password))
            {
                return new ValidationResult("Password must be atleast one numeric digit.");
            }
            if (!hasUpperCase.IsMatch(password))
            {
                return new ValidationResult("Password must be atleast one uppercase character.");
            }
            if (!hasLowerCase.IsMatch(password))
            {
                return new ValidationResult("Password must be atleast one lowercase character.");
            }
            if (!hasMinMaxCharacter.IsMatch(password))
            {
                return new ValidationResult("Password must be atleast 8 character.");
            }
            if (!hasSpecialCharacter.IsMatch(password))
            {
                return new ValidationResult("Password must be atleast one special character digit.");
            }

            return ValidationResult.Success;
        }
    }
}
