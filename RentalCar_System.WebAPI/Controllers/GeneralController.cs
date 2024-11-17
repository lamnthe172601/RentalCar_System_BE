
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
using Microsoft.Extensions.Caching.Memory;

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
        private readonly IMemoryCache _memoryCache;
        public GeneralController(
            RentalCarDBContext context,
            IConfiguration configuration,
            IAuthService authService,
            IUserService userService,
            IWebHostEnvironment environment,
            INotificationService notificationService,
             IMemoryCache memoryCache
            )
        {

            _dbContext = context;
            _configuration = configuration;
            _authService = authService;
            _userService = userService;
            _environment = environment;
            _notificationService = notificationService;
            _memoryCache = memoryCache;
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

            if (!_memoryCache.TryGetValue($"EmailVerified_{user.Email}", out bool isEmailVerified) || !isEmailVerified)
            {
                return BadRequest(new { message = "Email not verified. Please check your email." });
            }

            var token = _authService.GenerateJwtToken(user);

            // Return token
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
                return BadRequest(new { message = "Email already exists" });
            }
            if (await PhoneExists(model.PhoneNumber))
            {
                return BadRequest(new { message = "Phone number already exists" });
            }
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Email = model.Email,
                Password = HashPassword(model.Password),
                PhoneNumber = model.PhoneNumber,
                Name = model.Name,
                CreatedAt = DateTime.UtcNow // Set the CreatedAt property
            };
            _dbContext.Add(user);
            await _dbContext.SaveChangesAsync();

            // Generate verification token
            var verificationToken = Guid.NewGuid().ToString();
            var token = new Token
            {
                TokenId = Guid.NewGuid(),
                UserId = user.UserId,
                Token1 = verificationToken,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                IsUsed = false
            };
            _dbContext.Tokens.Add(token);
            await _dbContext.SaveChangesAsync();

            // Send verification email
            var frontendBaseUrl = _configuration["Frontend:BaseUrl"];
            var verificationLink = $"{frontendBaseUrl}/verify-email?token={verificationToken}&email={user.Email}";
            await _notificationService.SendEmailNotificationAsync(user.Email, "Verify your email", $"Please verify your email by clicking <a href='{verificationLink}'>here</a>.");

            return Ok(new { message = "Registration successful. Please check your email to verify your account." });
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

            if (!_memoryCache.TryGetValue($"EmailVerified_{user.Email}", out bool isEmailVerified) || !isEmailVerified)
            {
                return BadRequest(new { message = "Email not verified. Please check your email." });
            }

            // Check the last email sent time from MemoryCache
            if (_memoryCache.TryGetValue($"LastEmailSentAt_{user.Email}", out DateTime lastEmailSentAt))
            {
                if ((DateTime.UtcNow - lastEmailSentAt).TotalMinutes < 5)
                {
                    return BadRequest(new { message = "You can only request a password reset once every 5 minutes." });
                }
            }

            var token = GenerateResetToken();
            var resetToken = new Token
            {
                TokenId = Guid.NewGuid(),
                UserId = user.UserId,
                Token1 = token,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                IsUsed = false
            };

            _dbContext.Tokens.Add(resetToken);
            await _dbContext.SaveChangesAsync();

            var frontendBaseUrl = _configuration["Frontend:BaseUrl"];
            var resetLink = $"{frontendBaseUrl}/reset-password?token={token}";
            await _notificationService.SendEmailNotificationAsync(user.Email, "Reset Password", $"Click here to reset your password: {resetLink}");

            // Update the last email sent time in MemoryCache
            _memoryCache.Set($"LastEmailSentAt_{user.Email}", DateTime.UtcNow, TimeSpan.FromMinutes(5));

            return Ok(new { message = "Reset password link has been sent to your email." });
        }




        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var resetToken = await _dbContext.Tokens.FirstOrDefaultAsync(t => t.Token1 == request.Token && !t.IsUsed);
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
            _dbContext.Tokens.Update(resetToken);
            await _dbContext.SaveChangesAsync();
                
            return Ok(new { message = "Password has been reset successfully." });
        }


        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { message = "Invalid data provided", errors });
            }

            var token = await _dbContext.Tokens.FirstOrDefaultAsync(t => t.Token1 == request.Token && !t.IsUsed);
            if (token == null || token.ExpiresAt < DateTime.UtcNow)
            {
                return BadRequest(new { message = "Invalid or expired token." });
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == token.UserId);
            if (user == null)
            {
                return BadRequest(new { message = "User not found." });
            }

            user.IsEmailConfirmed = true;

            _dbContext.Users.Update(user);
            _dbContext.Tokens.Remove(token); // Remove the token from the database
            await _dbContext.SaveChangesAsync();

            _memoryCache.Set($"EmailVerified_{user.Email}", true, TimeSpan.FromDays(30)); // Cache verification status for 30 days

            return Ok(new { message = "Email verified successfully." });
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

