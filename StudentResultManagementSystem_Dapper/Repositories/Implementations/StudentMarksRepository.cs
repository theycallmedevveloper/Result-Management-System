using Dapper;
using Microsoft.Data.SqlClient;
using StudentResultManagementSystem_Dapper.DTOs;
using StudentResultManagementSystem_Dapper.Models;
using StudentResultManagementSystem_Dapper.Repositories.Interfaces;
using System.Data;

namespace StudentResultManagementSystem_Dapper.Repositories.Implementations
{
    public class StudentMarksRepository : IStudentMarksRepository
    {
        private readonly IConfiguration _config;

        public StudentMarksRepository(IConfiguration config)
        {
            _config = config;
        }

        private IDbConnection Connection =>
            new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        public void AddMarks(int studentId, int subjectId, int marks)
        {
            using var db = Connection;

            db.Execute(@"
                INSERT INTO StudentMarks (StudentId, SubjectId, MarksObtained)
                VALUES (@StudentId, @SubjectId, @MarksObtained)",
                new { StudentId = studentId, SubjectId = subjectId, MarksObtained = marks });
        }

        public bool UpdateMarks(int markId, int marks)
        {
            using var db = Connection;

            return db.Execute(@"
                UPDATE StudentMarks
                SET MarksObtained = @MarksObtained
                WHERE MarkId = @MarkId",
                new { MarkId = markId, MarksObtained = marks }) > 0;
        }

        public bool DeleteMarks(int markId)
        {
            using var db = Connection;

            return db.Execute(
                "DELETE FROM StudentMarks WHERE MarkId = @MarkId",
                new { MarkId = markId }) > 0;
        }

        public IEnumerable<StudentResultView> GetResultByStudentId(int studentId)
        {
            using var db = Connection;

            return db.Query<StudentResultView>(@"
                SELECT 
                    s.SubjectName,
                    s.MaxMarks,
                    sm.MarksObtained
                FROM StudentMarks sm
                INNER JOIN Subjects s ON sm.SubjectId = s.SubjectId
                WHERE sm.StudentId = @StudentId",
                new { StudentId = studentId });
        }


        public IEnumerable<AdminResultDto> GetAllResults()
        {
            using var db = Connection;

            return db.Query<AdminResultDto>(@"
        SELECT 
            st.FullName,
            sub.SubjectName,
            sm.MarksObtained
        FROM StudentMarks sm
        INNER JOIN Students st ON sm.StudentId = st.StudentId
        INNER JOIN Subjects sub ON sm.SubjectId = sub.SubjectId
    ");
        }

    }
}
