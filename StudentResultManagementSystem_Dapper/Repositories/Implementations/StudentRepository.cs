using Dapper;
using Microsoft.Data.SqlClient;
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
        )
    ", new { Query = query });
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
            OR FullName LIKE '%' + @Query + '%'
    ", new { Query = query });
        }


        public Student? GetByUserId(int userId)
        {
            using var db = Connection;

            return db.QueryFirstOrDefault<Student>(
                "SELECT * FROM Students WHERE UserId = @UserId AND IsActive = 1",
                new { UserId = userId }
            );
        }

        public int Create(Student student)
        {
            using var db = Connection;
            var sql = @"
            INSERT INTO Students (FirstName, LastName, Email, IsActive)
            VALUES (@FirstName, @LastName, @Email, 1);
            SELECT CAST(SCOPE_IDENTITY() as int);";

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
            WHERE StudentId = @StudentId", student);

            return rows > 0;
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