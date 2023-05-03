namespace CourseWork.Models;

public class User
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public AccessLevel AccessLevel { get; set; }
}