using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using portal_backend.Enums;

namespace portal_backend.Entities;

public class Customer
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public DateTime? LastLogin { get; set; }
    public string? LastOrderedService { get; set; }
    public bool IsBlocked { get; set; }
    public string? BanReason { get; set; }
    public LoyaltyGroup? LoyaltyGroup { get; set; }
    public User User { get; set; } = null!;
    public ICollection<Order>? Orders { get; set; }
}