using StudentResultManagementSystem_Dapper.Models;

namespace StudentResultManagementSystem_Dapper.Repositories.Interfaces
{
    public interface ISubjectRepository
    {
        IEnumerable<Subject> GetAll();
    }
}