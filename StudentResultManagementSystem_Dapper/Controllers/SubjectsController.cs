using Microsoft.AspNetCore.Mvc;
using StudentResultManagementSystem_Dapper.Repositories.Interfaces;

namespace StudentResultManagementSystem_Dapper.Controllers
{
    [ApiController]
    [Route("api/subjects")]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectRepository _repo;

        public SubjectsController(ISubjectRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_repo.GetAll());
        }
    }
}
