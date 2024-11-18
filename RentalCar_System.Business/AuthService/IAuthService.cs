using RentalCar_System.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalCar_System.Business.AuthService
{
    public interface IAuthService
    {
       string GenerateJwtToken(User user);

    }
}
