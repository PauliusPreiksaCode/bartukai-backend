using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace portal_backend.Entities;

public class Room
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
    public int Floor { get; set; }
    public int Accommodates { get; set; }
    public string Description { get; set; } = null!;
    public bool IsAvailable { get; set; }
    public ICollection<FullOrder>? FullOrder { get; set; }
}