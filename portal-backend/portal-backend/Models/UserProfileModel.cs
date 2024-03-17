using portal_backend.Enums;

namespace portal_backend.Models;

public class UserProfileModel
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? City { get; set; } = null!;
    public DateTime? BirthDate { get; set; }
    public Gender? Gender { get; set; }
    public string? PhoneNumber { get; set; } = null!;
    public DateTime CreationTime { get; set; }
}