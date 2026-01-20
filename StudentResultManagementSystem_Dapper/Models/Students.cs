public class Student
{
    public int StudentId { get; set; }
    public int UserId { get; set; } 

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool IsActive { get; set; }
}
