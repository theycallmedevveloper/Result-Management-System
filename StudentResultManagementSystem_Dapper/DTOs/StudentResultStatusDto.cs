namespace StudentResultManagementSystem_Dapper.DTOs
{
    public class StudentResultStatusDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public bool IsPassed { get; set; }
        public int TotalMarks { get; set; }
        public double Percentage { get; set; }
    }
}
