using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using portal_backend.Enums;

namespace portal_backend.Entities;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;

    [JsonIgnore]
    public string Password { get; set; } = null!;

    [JsonIgnore]
    public string Salt { get; set; } = null!;
    public string? City { get; set; } = null!;
    public DateTime? BirthDate { get; set; }
    public Gender? Gender { get; set; }
    public string? PhoneNumber { get; set; } = null!;
    public DateTime CreationTime { get; set; }
}