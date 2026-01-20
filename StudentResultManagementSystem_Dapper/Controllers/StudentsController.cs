using Microsoft.AspNetCore.Mvc;
using StudentResultManagementSystem_Dapper.DTOs;
using StudentResultManagementSystem_Dapper.Models;
using StudentResultManagementSystem_Dapper.Repositories.Interfaces;

namespace StudentResultManagementSystem_Dapper.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentRepository _repo;

        public StudentsController(IStudentRepository repo)
        {
            _repo = repo;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Role") == "Admin";
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

        [HttpPost("create")]
        public IActionResult Create(StudentCreateDto dto)
        {
            if (!IsAdmin())
                return Forbid("Only admin can create students");

            var student = new Student
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email
            };

            var id = _repo.Create(student);
            return Ok(new { StudentId = id });
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

        [HttpGet("delete/{id}")]
        public IActionResult Delete(int id)
        {
            if (!IsAdmin())
                return Forbid("Only admin can delete students");

            return _repo.Delete(id) ? Ok() : NotFound();
        }
    }
}
