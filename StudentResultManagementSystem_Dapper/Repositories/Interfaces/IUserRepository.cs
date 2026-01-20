using StudentResultManagementSystem_Dapper.Models;

namespace StudentResultManagementSystem_Dapper.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> Login(string username, string password);
    }
}
