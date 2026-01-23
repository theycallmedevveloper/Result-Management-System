using StudentResultManagementSystem_Dapper.Models;

namespace StudentResultManagementSystem_Dapper.Repositories.Interfaces
{
    public interface IStudentRepository
    {
        IEnumerable<Student> SearchStudents(string query);

        Student? GetById(int id);
        Student? GetByUserId(int userId);

        Student? Search(string query);

        int Create(Student student);
        bool Update(Student student);
        bool Delete(int id);
    }
}
