using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using StudentResultManagementSystem_Dapper.Models;
using StudentResultManagementSystem_Dapper.Repositories.Interfaces;
using System.Data;

namespace StudentResultManagementSystem_Dapper.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly IConfiguration _config;

        public UserRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        private IDbConnection Connection =>
            new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        public User GetByUserId(int userId)
        {
            using var db = Connection;

            return db.QueryFirstOrDefault<User>(
                "SELECT * FROM Users WHERE UserId = @UserId",
                new { UserId = userId }
            );
        }

        public async Task<User?> Login(string username, string password)
        {
            using var db = Connection;

            var sql = @"
                SELECT UserId, Username, Role
                FROM Users
                WHERE Username = @Username
                  AND Password = @Password
                  AND IsActive = 1";

            return await db.QueryFirstOrDefaultAsync<User>(
                sql,
                new { Username = username, Password = password }
            );
        }
    }
}
