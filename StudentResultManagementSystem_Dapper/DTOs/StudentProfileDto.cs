namespace StudentResultManagementSystem_Dapper.DTOs
{
    public class StudentProfileDto
    {
        public int StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RollNumber { get; set; }
        public string Class { get; set; }
        public string Email { get; set; }
        public string ProfilePhotoUrl { get; set; }
    }
}
