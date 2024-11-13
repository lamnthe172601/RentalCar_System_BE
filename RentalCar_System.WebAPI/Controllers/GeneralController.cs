using RentalCar_System.Business.BaseService;
using Microsoft.AspNetCore.Http;
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


        public GeneralController(RentalCarDBContext context, IConfiguration configuration, IAuthService authService,IUserService userService)
        {
            _dbContext = context;
            _configuration = configuration;
            _authService = authService;
            _userService = userService;
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
            { var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { message = "Invalid data provided", errors }); }
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
    }
}
