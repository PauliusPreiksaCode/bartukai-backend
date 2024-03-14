using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace portal_backend.Entities;

public class Specialist
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string? Photo { get; set; }
    public string? Description { get; set; }
    public string? Experience { get; set; }
    public string? Education { get; set; }
    public string? AgreementId { get; set; }
    public User User { get; set; } = null!;
    public ICollection<Service>? Services { get; set; }
}