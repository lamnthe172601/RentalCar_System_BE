using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalCar_System.Models.DtoViewModel
{
    public class UserProfileViewModel
    {
        public string Name { get; set; }=string.Empty;

        public string Email { get; set; } = string.Empty;       

        public string PhoneNumber { get; set; } = string.Empty;

        public byte[] Photo { get; set; }

    }
}
