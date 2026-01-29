using StudentResultManagementSystem_Dapper.DTOs;
using StudentResultManagementSystem_Dapper.Models;

namespace StudentResultManagementSystem_Dapper.Repositories.Interfaces
{
    public interface IStudentMarksRepository
    {
        void AddOrUpdateMarks(int studentId, int subjectId, int marks);

        bool UpdateMarks(int markId, int marks);
        bool DeleteMarks(int markId);

        IEnumerable<StudentResultView> GetResultByStudentId(int studentId);
        IEnumerable<AdminResultDto> GetAllResults();
    }
}