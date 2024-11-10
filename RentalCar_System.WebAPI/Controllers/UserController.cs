using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RentalCar_System.Business.UserService;
using RentalCar_System.Models.DtoViewModel;
using RentalCar_System.Models.Entity;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RentalCar_System.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="admin")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userRepository;

        public UserController(IUserService userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("User")]
        public async Task<IActionResult> GetAllUser()
        {
            var User = await _userRepository.GetAllUsersAsync();
            return Ok(User);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var User = await _userRepository.GetUserByIdAsync(id);
            if (User == null) return NotFound();
            return Ok(User);
        }        

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, UpdateUserViewModel model)
        {
           var user =await _userRepository.GetUserByIdAsync(id);
            if (user == null) return BadRequest(new { message = "Not found!"});
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            await _userRepository.UpdateUserAsync(user);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            await _userRepository.DeleteUserAsync(id);
            return NoContent();
        }
    }
}
