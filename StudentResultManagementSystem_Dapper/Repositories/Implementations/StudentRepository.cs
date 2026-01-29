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
        private string _connectionString;

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
            OR FullName LIKE '%' + @Query + '%'
    ", new { Query = query });
        }

        public Student GetByRollNumber(string rollNumber)
        {
            using var connection = new SqlConnection(_connectionString);
            return connection.QueryFirstOrDefault<Student>(
                "SELECT * FROM Students WHERE RollNumber = @RollNumber",
                new { RollNumber = rollNumber });
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
        INSERT INTO Students (
            FirstName, LastName, Email,
            RollNumber, Class,          -- ← add these
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
            WHERE StudentId = @StudentId", student);

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
                )
                ";
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