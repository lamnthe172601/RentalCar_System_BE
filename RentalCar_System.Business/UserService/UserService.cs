using RentalCar_System.Data;
using RentalCar_System.Data.UserRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using RentalCar_System.Models.Entity;

namespace RentalCar_System.Business.UserService
{
    public class UserService : IUserService
    {

        private readonly IUserRepository _userRepository;


        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        #region Customer
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return (await _userRepository.GetAllAsync()).Where(u => u.Role.ToLower() == "customer");
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user.Role.ToLower() == "customer" ? user : null;
        }

        public async Task<User> AddUserAsync(User user)
        {
            if (user.Role.ToLower() == "customer")
            {
                throw new Exception("User role must be User");
            }
            await _userRepository.AddAsync(user);
            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            if (user.Role.ToLower() == "customer")
            {
                throw new Exception("User role must be User");
            }
            await _userRepository.UpdateAsync(user);
            return user;
        }

        public async Task<User> DeleteUserAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user.Role.ToLower() == "customer")
            {
                throw new Exception("User role must be User");
            }
            if (user != null)
            {
                await _userRepository.DeleteAsync(id);
            }
            return user;
        }
        #endregion

       
    }

}
