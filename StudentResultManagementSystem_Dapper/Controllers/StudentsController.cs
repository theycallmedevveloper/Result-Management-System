using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;   
using StudentResultManagementSystem_Dapper.DTOs;
using StudentResultManagementSystem_Dapper.Models;
using StudentResultManagementSystem_Dapper.Repositories.Implementations;
using StudentResultManagementSystem_Dapper.Repositories.Interfaces;

namespace StudentResultManagementSystem_Dapper.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentRepository _repo;
        private readonly IConfiguration _configuration;   
        public StudentsController(
            IStudentRepository repo,
            IConfiguration configuration)   
        {
            _repo = repo;
            _configuration = configuration;
        }

        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("Role");
            Console.WriteLine($"Current user role: {role}"); // Debug
            return role == "Admin";
        }

        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            if (!IsAdmin())
                return Forbid("Only admin can view all students");

            return Ok(_repo.GetAll());
        }

        [HttpGet("get/{id}")]
        public IActionResult Get(int id)
        {
            if (!IsAdmin())
                return Forbid();

            var student = _repo.GetById(id);
            return student == null ? NotFound() : Ok(student);
        }

        [HttpGet("suggest")]
        public IActionResult Suggest(string q)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return StatusCode(403);

            if (string.IsNullOrWhiteSpace(q))
                return Ok(new List<object>());

            var students = _repo.SuggestStudents(q);
            return Ok(students);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] StudentCreateDto dto)
        {
            if (!IsAdmin())
                return Forbid("Only admin can create students");

            if (string.IsNullOrWhiteSpace(dto.FirstName) ||
                string.IsNullOrWhiteSpace(dto.LastName) ||
                string.IsNullOrWhiteSpace(dto.Class) ||
                string.IsNullOrWhiteSpace(dto.RollNumber) ||  
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest("All fields are required.");
            }

            // IMPORTANT: Check for NULL or empty RollNumber in code
            if (string.IsNullOrWhiteSpace(dto.RollNumber))
            {
                return BadRequest("RollNumber cannot be empty.");
            }

            var username = $"{dto.FirstName.ToLower()}.{dto.RollNumber}".Replace(" ", "");

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            // Create User first
            var userSql = @"
        INSERT INTO Users (Username, Password, Role, IsActive)
        VALUES (@Username, @Password, 'Student', 1);
        SELECT CAST(SCOPE_IDENTITY() AS int);";

            int userId;
            try
            {
                userId = await connection.ExecuteScalarAsync<int>(
                    userSql,
                    new
                    {
                        Username = username,
                        Password = dto.Password
                    });
            }
            catch (SqlException ex) when (ex.Number is 2627 or 2601)
            {
                return Conflict("Username already exists.");
            }

            // Create Student - Use parameterized query to avoid NULL issue
            var studentSql = @"
        INSERT INTO Students 
        (FirstName, LastName, FullName, Email, UserId, Class, RollNumber, IsActive)
        VALUES 
        (@FirstName, @LastName, @FullName, @Email, @UserId, @Class, @RollNumber, 1);
        SELECT CAST(SCOPE_IDENTITY() AS int);";

            try
            {
                var studentId = await connection.ExecuteScalarAsync<int>(studentSql, new
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    FullName = $"{dto.FirstName} {dto.LastName}",
                    Email = dto.Email,
                    UserId = userId,
                    Class = dto.Class,
                    RollNumber = dto.RollNumber  
                });

                return Ok(new
                {
                    studentId,
                    username,
                    message = "Student created successfully"
                });
            }
            catch (SqlException ex)
            {
                // Log the full error
                Console.WriteLine($"SQL Error: {ex.Message}");
                Console.WriteLine($"Error Number: {ex.Number}");

                if (ex.Number == 2627 || ex.Number == 2601)  
                {
                    if (ex.Message.Contains("RollNumber"))
                    {
                        return Conflict($"Roll number '{dto.RollNumber}' already exists.");
                    }
                    return Conflict("Duplicate entry. Please check the data.");
                }

                return StatusCode(500, $"Database error: {ex.Message}");
            }
        }

        [HttpPost("update")]
        public IActionResult Update(StudentUpdateDto dto)
        {
            if (!IsAdmin())
                return Forbid("Only admin can update students");

            var success = _repo.Update(new Student
            {
                StudentId = dto.StudentId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email
            });

            return success ? Ok() : NotFound();
        }

        [HttpPost("upload-profile-photo")]
        public async Task<IActionResult> UploadProfilePhoto(IFormFile photo)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Unauthorized();

            var student = _repo.GetByUserId(userId.Value);
            if (student == null)
                return Unauthorized();

            if (photo == null || photo.Length == 0)
                return BadRequest("No file uploaded");

            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var fileName = $"student_{student.StudentId}{Path.GetExtension(photo.FileName)}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }

            var photoUrl = $"/uploads/{fileName}";

            _repo.UpdateProfilePhoto(student.StudentId, photoUrl);

            return Ok(new { profilePhotoUrl = photoUrl });
        }



        [Authorize(Roles = "Student")]
        [HttpGet("{id}/result-status")]
        public async Task<IActionResult> GetResultStatus(int id)
        {
            var result = await _repo.GetStudentResultStatusAsync(id);
            if (result == null)
                return NotFound("Result not found");

            return Ok(result);
        }

        [HttpGet("{id}/profile")]
        public async Task<IActionResult> GetProfile(int id)
        {
            var profile = await _repo.GetStudentProfileAsync(id);
            if (profile == null)
                return NotFound();

            return Ok(profile);
        }


        [HttpGet("delete/{id}")]
        public IActionResult Delete(int id)
        {
            if (!IsAdmin())
                return Forbid("Only admin can delete students");

            return _repo.Delete(id) ? Ok() : NotFound();
        }
    }
}