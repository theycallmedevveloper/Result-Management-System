using StudentResultManagementSystem_Dapper.Models;

namespace StudentResultManagementSystem_Dapper.Repositories.Interfaces
{
    public interface IStudentRepository
    {
        IEnumerable<Student> GetAll();
        Student? GetById(int id);
        int Create(Student student);
        bool Update(Student student);
        bool Delete(int id);
    }
}
