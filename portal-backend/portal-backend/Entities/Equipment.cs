using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace portal_backend.Entities;

public class Equipment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int? InventoryNumber { get; set; }
    public string Name { get; set; } = null!;
    public string? Condition { get; set; }
    public string? Type { get; set; }
    public bool IsAvailable { get; set; }
    public ICollection<FullOrder>? FullOrders { get; set; }
}