using Microsoft.AspNetCore.Mvc;
using StudentResultManagementSystem_Dapper.DTOs;
using StudentResultManagementSystem_Dapper.Repositories.Interfaces;

namespace StudentResultManagementSystem_Dapper.Controllers
{
    [ApiController]
    [Route("api/marks")]
    public class StudentMarksController : ControllerBase
    {
        private readonly IStudentMarksRepository _marksRepo;
        private readonly IStudentRepository _studentRepo;

        public StudentMarksController(
            IStudentMarksRepository marksRepo,
            IStudentRepository studentRepo)
        {
            _marksRepo = marksRepo;
            _studentRepo = studentRepo;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Role") == "Admin";
        }

        // ADMIN ONLY
        [HttpPost("add")]
        public IActionResult AddMarks(StudentMarkCreateDto dto)
        {
            if (!IsAdmin())
                return Forbid("Only admin can add marks");

            _marksRepo.AddMarks(dto.StudentId, dto.SubjectId, dto.MarksObtained);
            return Ok("Marks added");
        }

        // ADMIN ONLY
        [HttpPost("update")]
        public IActionResult UpdateMarks(StudentMarkUpdateDto dto)
        {
            if (!IsAdmin())
                return Forbid();

            return _marksRepo.UpdateMarks(dto.MarkId, dto.MarksObtained)
                ? Ok("Marks updated")
                : NotFound();
        }

        // ADMIN ONLY
        [HttpDelete("delete/{markId}")]
        public IActionResult DeleteMarks(int markId)
        {
            if (!IsAdmin())
                return Forbid();

            return _marksRepo.DeleteMarks(markId)
                ? Ok("Marks deleted")
                : NotFound();
        }

        // STUDENT ONLY
        [HttpGet("my-result")]
        public IActionResult MyResult()
        {
            if (HttpContext.Session.GetString("Role") != "Student")
                return Forbid();

            var userId = HttpContext.Session.GetInt32("UserId");

            var student = _studentRepo.GetByUserId(userId!.Value);
            if (student == null)
                return NotFound();

            return Ok(_marksRepo.GetResultByStudentId(student.StudentId));
        }

        // ADMIN ONLY
        [HttpGet("all-results")]
        public IActionResult AllResults()
        {
            if (!IsAdmin())
                return Forbid();

            return Ok(_marksRepo.GetAllResults());
        }
    }
}
