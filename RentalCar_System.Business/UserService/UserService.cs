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
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;

namespace RentalCar_System.Business.UserService
{
    public class UserService : IUserService
    {

        private readonly IUserRepository _userRepository;

        private readonly IWebHostEnvironment _environment;
        public UserService(IUserRepository userRepository, IWebHostEnvironment environment)
        {
            _userRepository = userRepository;
            _environment = environment;
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

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<string> UpdateUserAvatarAsync(Guid userId, IFormFile avatarFile)
        {
            if (avatarFile == null || avatarFile.Length == 0)
                throw new ArgumentException("No file uploaded");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            var uploadsFolder = Path.Combine(_environment.ContentRootPath, "Images", "image-profile");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Xóa ảnh đại diện cũ nếu có
            if (!string.IsNullOrEmpty(user.PhotoUrl))
            {
                var oldAvatarPath = Path.Combine(_environment.ContentRootPath, user.PhotoUrl);
                if (File.Exists(oldAvatarPath))
                {
                    File.Delete(oldAvatarPath);
                }
            }

            // Tạo tên tệp với userId
            var fileName = $"{userId}{Path.GetExtension(avatarFile.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Lưu tệp mới
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await avatarFile.CopyToAsync(fileStream);
            }

            user.PhotoUrl = Path.Combine("Images", "image-profile", fileName);
            await _userRepository.UpdateAsync(user);

            return user.PhotoUrl;
        }

    }
    #endregion


}


