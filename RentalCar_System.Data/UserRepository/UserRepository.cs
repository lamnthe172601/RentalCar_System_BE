using Microsoft.EntityFrameworkCore;
using RentalCar_System.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalCar_System.Data.UserRepository
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly RentalCarDBContext _context;
        public UserRepository(RentalCarDBContext context) : base(context)
        {
            _context = context;

        }
       
    }
}
