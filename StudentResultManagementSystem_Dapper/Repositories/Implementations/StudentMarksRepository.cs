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

        public void AddOrUpdateMarks(int studentId, int subjectId, int marks)
        {
            using var db = Connection;

            var exists = db.ExecuteScalar<int>(@"
        SELECT COUNT(1)
        FROM StudentMarks
        WHERE StudentId = @StudentId AND SubjectId = @SubjectId",
                new { StudentId = studentId, SubjectId = subjectId });

            if (exists > 0)
            {
                db.Execute(@"
            UPDATE StudentMarks
            SET MarksObtained = @MarksObtained
            WHERE StudentId = @StudentId AND SubjectId = @SubjectId",
                    new { StudentId = studentId, SubjectId = subjectId, MarksObtained = marks });
            }
            else
            {
                db.Execute(@"
            INSERT INTO StudentMarks (StudentId, SubjectId, MarksObtained)
            VALUES (@StudentId, @SubjectId, @MarksObtained)",
                    new { StudentId = studentId, SubjectId = subjectId, MarksObtained = marks });
            }
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

            var sql = @"
            SELECT
                s.StudentId AS StudentId,
                CONCAT(s.FirstName, ' ', s.LastName) AS StudentName,
                sub.SubjectName AS SubjectName,
                sm.MarksObtained AS MarksObtained
            FROM StudentMarks sm
            INNER JOIN Students s ON sm.StudentId = s.StudentId
            INNER JOIN Subjects sub ON sm.SubjectId = sub.SubjectId
            ";

            return db.Query<AdminResultDto>(sql);
        }

    }
}
