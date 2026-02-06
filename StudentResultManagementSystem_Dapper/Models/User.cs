using System.Security.Cryptography.X509Certificates;

namespace StudentResultManagementSystem_Dapper.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password{ get; set; }
        public string Role { get; set; }

    }
}
