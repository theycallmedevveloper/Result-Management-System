using StudentResultManagementSystem_Dapper.DTOs;
using StudentResultManagementSystem_Dapper.Models;

namespace StudentResultManagementSystem_Dapper.Repositories.Interfaces
{
    public interface IStudentRepository
    {
        IEnumerable<Student> SearchStudents(string query);

        Task<StudentResultStatusDto> GetStudentResultStatusAsync(int studentId);

        Task<StudentProfileDto> GetStudentProfileAsync(int studentId);

        IEnumerable<Student> GetAll();
        Student? GetById(int id);
        Student? GetByUserId(int userId);

        Student GetByRollNumber(string rollNumber);
        Student? Search(string query);

        IEnumerable<object> SuggestStudents(string query);

        void UpdateProfilePhoto(int studentId, string photoUrl);

        int Create(Student student);
        bool Update(Student student);
        bool Delete(int id);
    }
}
