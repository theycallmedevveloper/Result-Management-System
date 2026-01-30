using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentResultManagementSystem_Dapper.DTOs;
using StudentResultManagementSystem_Dapper.Repositories.Implementations;
using StudentResultManagementSystem_Dapper.Repositories.Interfaces;

namespace StudentResultManagementSystem_Dapper.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IStudentRepository _studentRepository;

        public AuthController(
             IUserRepository userRepository,
             IStudentRepository studentRepository)
        {
            _userRepository = userRepository;
            _studentRepository = studentRepository;
        }

        [HttpGet("profile")]
        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role == null)
                return Unauthorized();

            var user = _userRepository.GetByUserId(userId.Value);
            if (user == null)
                return Unauthorized();

            int? studentId = null;

            if (role == "Student")
            {
                var student = _studentRepository.GetByUserId(userId.Value);
                if (student != null)
                    studentId = student.StudentId;
            }

            return Ok(new
            {
                userId = user.UserId,
                username = user.Username,
                role = role,
                studentId = studentId
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
