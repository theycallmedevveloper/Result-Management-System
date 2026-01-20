using Microsoft.AspNetCore.Mvc;
using StudentResultManagementSystem_Dapper.DTOs;
using StudentResultManagementSystem_Dapper.Repositories.Interfaces; 

namespace StudentResultManagementSystem_Dapper.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("profile")]

        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return Unauthorized("Please Log in First");

            return Ok(new
            {
                UserId = userId,
                Username = HttpContext.Session.GetString("Username"),
                Role = HttpContext.Session.GetString("Role")
            });
        }

        [HttpPost("login")]

        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                return BadRequest("Username and Password are required");

            var user = await _userRepository.Login(request.Username, request.Password);

            if (user == null)   
                return Unauthorized("Invalid Username and Password");

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role);

            return Ok("Login successful");

        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok("Logged out successfully");
        }

    }
}
