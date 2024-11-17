
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RentalCar_System.Data;
using RentalCar_System.Models.DtoViewModel;
using RentalCar_System.Models.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using RentalCar_System.Business.AuthService;
using RentalCar_System.Business.UserService;
using System.Security.Cryptography;
using RentalCar_System.Business.NotificationService;

namespace RentalCar_System.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeneralController : ControllerBase
    {
        private readonly RentalCarDBContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _environment;
        private readonly INotificationService _notificationService;
        public GeneralController(
            RentalCarDBContext context,
            IConfiguration configuration,
            IAuthService authService,
            IUserService userService,
            IWebHostEnvironment environment,
            INotificationService notificationService
            )
        {

            _dbContext = context;
            _configuration = configuration;
            _authService = authService;
            _userService = userService;
            _environment = environment;
            _notificationService = notificationService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { message = "Invalid data provided", errors });
            }

            var user = await _userService.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest(new { message = "Invalid email or password" });
            }

            if (!VerifyPassword(model.Password, user.Password))
            {
                return BadRequest(new { message = "Invalid password" });
            }

            var token = _authService.GenerateJwtToken(user);

            // Trả về token
            return Ok(new { Token = token });
        }

        private bool VerifyPassword(string? passwordFE, string? passwordBE)
        {
            return BCrypt.Net.BCrypt.Verify(passwordFE, passwordBE);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { message = "Invalid data provided", errors });
            }
            if (await EmailExists(model.Email))
            {
                return BadRequest(new { message = "Email already Exsist" });
            }
            if (await PhoneExists(model.PhoneNumber))
            {
                return BadRequest(new { message = "PhoneNumber already Exsist" });
            }
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Email = model.Email,
                Password = HashPassword(model.Password),
                PhoneNumber = model.PhoneNumber,
                Name = model.Name,

            };
            _dbContext.Add(user);
            _dbContext.SaveChanges();

            return Ok(new { message = "Registration  successfully" });
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { message = "Invalid data provided", errors });
            }
            var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            if (emailClaim == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email.ToLower() == emailClaim.Value.ToLower());
            if (user == null)
            {
                return BadRequest(new { message = "User not found" });
            }
            if (!VerifyPassword(model.OldPassword, user.Password))
            {
                return BadRequest(new { message = "Invalid old password" });
            }
            user.Password = HashPassword(model.NewPassword); _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
            return Ok(new { message = "Password changed successfully" });
        }
        private async Task<bool> PhoneExists(string phone)
        {
            return await _dbContext.Users.AnyAsync(user => user.PhoneNumber == phone);
        }

        private string HashPassword(string? password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        private async Task<bool> EmailExists(string email)
        {
            return await _dbContext.Users.AnyAsync(user => user.Email.ToLower() == email.ToLower());
        }

        [HttpGet("Get-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfileUser()
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { message = "Invalid data provided", errors });
            }
            var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            if (emailClaim == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email.ToLower() == emailClaim.Value.ToLower());
            if (user == null)
            {
                return BadRequest(new { message = "User not found" });
            }
            UserProfileViewModel model = new UserProfileViewModel
            {
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                PhotoUrl = user.PhotoUrl,
            };

            return Ok(model);
        }

        [HttpPut("update-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfileUser(UpdateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { message = "Invalid data provided", errors });
            }
            var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            if (emailClaim == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email.ToLower() == emailClaim.Value.ToLower());
            if (user == null)
            {
                return BadRequest(new { message = "User not found" });
            }
            user.Name = model.Name;
            if (user.PhoneNumber != model.PhoneNumber && await PhoneExists(model.PhoneNumber))
            {
                return BadRequest(new { message = "PhoneNumber already Exsist" });
            }
            user.PhoneNumber = model.PhoneNumber;
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
            return Ok(new { message = "Updated profile successfully" });
        }

        [HttpPost("upload-avatar")]
        [Authorize]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "No file uploaded" });
            }
            try
            {
                var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                if (emailClaim == null)
                {
                    return Unauthorized(new { message = "Invalid token" });
                }
                var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email.ToLower() == emailClaim.Value.ToLower());
                if (user == null)
                {
                    return BadRequest(new { message = "User not found" });
                }
                var userId = user.UserId;
                // Lấy userId từ token
                var filePath = await _userService.UpdateUserAvatarAsync(userId, file);
                return Ok(new { message = "File uploaded successfully", filePath });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("get-avatar")]
        [Authorize]
        public async Task<IActionResult> GetAvatar()
        {
            var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            if (emailClaim == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email.ToLower() == emailClaim.Value.ToLower());
            if (user == null)
            {
                return BadRequest(new { message = "User not found" });
            }

            var avatarPath = Path.Combine(_environment.ContentRootPath, user.PhotoUrl);
            if (!System.IO.File.Exists(avatarPath))
            {
                return NotFound(new { message = "Avatar file not found" });
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(avatarPath);
            return File(fileBytes, "image/jpeg"); // Or other image format if needed
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return BadRequest(new { message = "Email not found" });
            }

            var token = GenerateResetToken();
            var resetToken = new PasswordResetToken
            {
                TokenId = Guid.NewGuid(),
                UserId = user.UserId,
                Token = token,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                IsUsed = false
            };

            _dbContext.PasswordResetTokens.Add(resetToken);
            await _dbContext.SaveChangesAsync();

            var frontendBaseUrl = _configuration["Frontend:BaseUrl"];
            var resetLink = $"{frontendBaseUrl}/reset-password?token={token}";
            await _notificationService.SendEmailNotificationAsync(user.Email, "Reset Password", $"Click here to reset your password: {resetLink}");
            
            return Ok(new { message = "Reset password link has been sent to your email." });
        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var resetToken = await _dbContext.PasswordResetTokens.FirstOrDefaultAsync(t => t.Token == request.Token && !t.IsUsed);
            if (resetToken == null || resetToken.ExpiresAt < DateTime.UtcNow)
            {              
                return BadRequest(new { message = "Invalid or expired token." });
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == resetToken.UserId);
            if (user == null)
            {              
                return BadRequest(new { message = "User not found." });
            }

            user.Password = HashPassword(request.NewPassword);
            resetToken.IsUsed = true;

            _dbContext.Users.Update(user);
            _dbContext.PasswordResetTokens.Update(resetToken);
            await _dbContext.SaveChangesAsync();
                
            return Ok(new { message = "Password has been reset successfully." });
        }

        private string GenerateResetToken()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var tokenData = new byte[32];
                rng.GetBytes(tokenData);
                return Convert.ToBase64String(tokenData);
            }
        }

    }

}

