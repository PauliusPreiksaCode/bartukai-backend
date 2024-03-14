using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace portal_backend.Entities;

public class FullOrder
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int? OrderId { get; set; }
    public Order? Order { get; set; }
    public Service Service { get; set; } = null!;
    public Room? Room { get; set; }
    public ICollection<Equipment>? Equipment { get; set; }
}