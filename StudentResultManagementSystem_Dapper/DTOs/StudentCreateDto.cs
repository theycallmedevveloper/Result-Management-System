using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class StudentCreateDto
{
    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required]
    [JsonPropertyName("class")]  // Use this if your JSON uses lowercase "class"
    public string Class { get; set; }

    [Required]
    public string RollNumber { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}