using Dapper;
using Microsoft.Data.SqlClient;
using StudentResultManagementSystem_Dapper.Models;
using StudentResultManagementSystem_Dapper.Repositories.Interfaces;
using System.Data;

namespace StudentResultManagementSystem_Dapper.Repositories.Implementations
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly IConfiguration _config;

        public SubjectRepository(IConfiguration config)
        {
            _config = config;
        }

        private IDbConnection Connection =>
            new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        public IEnumerable<Subject> GetAll()
        {
            using var db = Connection;
            return db.Query<Subject>("SELECT SubjectId, SubjectName FROM Subjects");
        }
    }
}
