using Dapper;
using Microsoft.Data.SqlClient;
using StudentResultManagementSystem_Dapper.DTOs;
using StudentResultManagementSystem_Dapper.Models;
using StudentResultManagementSystem_Dapper.Repositories.Interfaces;
using System.Data;

namespace StudentResultManagementSystem_Dapper.Repositories.Implementations
{
    public class StudentRepository : IStudentRepository
    {
        private readonly IConfiguration _config;

        public StudentRepository(IConfiguration config)
        {
            _config = config;
        }

        private IDbConnection Connection =>
            new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        public IEnumerable<Student> SearchStudents(string query)
        {
            using var db = Connection;

            return db.Query<Student>(@"
                SELECT *
                FROM Students
                WHERE IsActive = 1
                AND (
                    FullName LIKE '%' + @Query + '%'
                    OR RollNumber LIKE '%' + @Query + '%'
                    OR CAST(StudentId AS NVARCHAR) LIKE '%' + @Query + '%'
                )",
                new { Query = query });
        }

        public IEnumerable<Student> GetAll()
        {
            using var db = Connection;
            return db.Query<Student>(
                "SELECT * FROM Students WHERE IsActive = 1");
        }

        public Student? GetById(int id)
        {
            using var db = Connection;
            return db.QueryFirstOrDefault<Student>(
                "SELECT * FROM Students WHERE StudentId = @Id",
                new { Id = id });
        }

        public Student? Search(string query)
        {
            using var db = Connection;

            return db.QueryFirstOrDefault<Student>(@"
                SELECT *
                FROM Students
                WHERE 
                    StudentId = TRY_CAST(@Query AS INT)
                    OR FullName LIKE '%' + @Query + '%'",
                new { Query = query });
        }

        public Student GetByRollNumber(string rollNumber)
        {
            using var db = Connection;

            return db.QueryFirstOrDefault<Student>(
                "SELECT * FROM Students WHERE RollNumber = @RollNumber",
                new { RollNumber = rollNumber });
        }

        public async Task<StudentProfileDto> GetStudentProfileAsync(int studentId)
        {
            using var db = Connection;

            var sql = @"
                SELECT 
                    StudentId,
                    FirstName,
                    LastName,
                    RollNumber,
                    Class,
                    Email,
                    ProfilePhotoUrl
                FROM Students
                WHERE StudentId = @StudentId";

            return await db.QueryFirstOrDefaultAsync<StudentProfileDto>(
                sql, new { StudentId = studentId });
        }

        public Student? GetByUserId(int userId)
        {
            using var db = Connection;

            return db.QueryFirstOrDefault<Student>(
                "SELECT * FROM Students WHERE UserId = @UserId AND IsActive = 1",
                new { UserId = userId });
        }

        public async Task<StudentResultStatusDto> GetStudentResultStatusAsync(int studentId)
        {
            using var db = Connection;

            var sql = @"
                SELECT 
                    s.StudentId,
                    CONCAT(s.FirstName, ' ', s.LastName) AS StudentName,
                    SUM(sm.MarksObtained) AS TotalMarks,
                    CASE 
                        WHEN MIN(sm.MarksObtained) >= 35 THEN 1
                        ELSE 0
                    END AS IsPassed,
                    CAST(SUM(sm.MarksObtained) * 100.0 / (COUNT(sm.MarkId) * 100) AS DECIMAL(5,2)) AS Percentage
                FROM Students s
                INNER JOIN StudentMarks sm ON s.StudentId = sm.StudentId
                WHERE s.StudentId = @StudentId
                GROUP BY s.StudentId, s.FirstName, s.LastName";

            return await db.QueryFirstOrDefaultAsync<StudentResultStatusDto>(
                sql,
                new { StudentId = studentId });
        }

        public void UpdateProfilePhoto(int studentId, string photoUrl)
        {
            using var db = Connection;

            db.Execute(
                "UPDATE Students SET ProfilePhotoUrl = @Url WHERE StudentId = @Id",
                new { Url = photoUrl, Id = studentId });
        }

        public int Create(Student student)
        {
            using var db = Connection;

            var sql = @"
                INSERT INTO Students (
                    FirstName, LastName, Email,
                    RollNumber, Class,
                    UserId, IsActive
                )
                VALUES (
                    @FirstName, @LastName, @Email,
                    @RollNumber, @Class,
                    @UserId, 1
                );
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return db.QuerySingle<int>(sql, student);
        }

        public bool Update(Student student)
        {
            using var db = Connection;

            var rows = db.Execute(@"
                UPDATE Students
                SET FirstName = @FirstName,
                    LastName = @LastName,
                    Email = @Email
                WHERE StudentId = @StudentId",
                student);

            return rows > 0;
        }

        public IEnumerable<object> SuggestStudents(string query)
        {
            using var db = Connection;

            var sql = @"
                SELECT
                    StudentId AS studentId,
                    FirstName AS firstName,
                    LastName AS lastName,
                    RollNumber AS rollNumber
                FROM Students
                WHERE IsActive = 1
                AND (
                    FirstName LIKE @q
                    OR LastName LIKE @q
                    OR RollNumber LIKE @q
                )";

            return db.Query(sql, new { q = $"%{query}%" });
        }

        public bool Delete(int id)
        {
            using var db = Connection;

            return db.Execute(
                "UPDATE Students SET IsActive = 0 WHERE StudentId = @Id",
                new { Id = id }) > 0;
        }
    }
}