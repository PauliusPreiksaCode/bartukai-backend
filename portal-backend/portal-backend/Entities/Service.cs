using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using portal_backend.Enums;

namespace portal_backend.Entities;

public class Service
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; } = null!;
    public decimal Price { get; set; }
    public bool IsVerified { get; set; }
    public string? Link { get; set; } = null!;
    public string? AddressDescription { get; set; } = null!;
    public string? Address { get; set; } = null!;
    public ServiceLocation ServiceLocation { get; set; }
    public bool IsAvailable { get; set; }
    public ICollection<ServiceCategory> ServiceCategories { get; set; } = null!;
    public Specialist Specialist { get; set; } = null!;
    public ICollection<FullOrder> FullOrders { get; set; } = null!;
}