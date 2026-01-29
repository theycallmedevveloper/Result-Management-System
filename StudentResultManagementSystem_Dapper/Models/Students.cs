namespace StudentResultManagementSystem_Dapper.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public int UserId { get; set; }
        public string? FullName { get; set; } 
        public string? RollNumber { get; set; }
        public string? Class { get; set; } 
        public bool IsActive { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
