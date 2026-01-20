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
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<User?> Login(string username, string password)
        {
            using var connection = new SqlConnection(_connectionString);

            var sql = @"
                SELECT UserId, Username, Role
                FROM Users
                WHERE Username = @Username
                  AND Password = @Password
                  AND IsActive = 1";

            return await connection.QueryFirstOrDefaultAsync<User>(
                sql,
                new { Username = username, Password = password }
            );
        }
    }
}
