using portal_backend.Entities;
using portal_backend.Enums;

namespace portal_backend.Models;

public class ServiceModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; } = null!;
    public decimal Price { get; set; }
    public bool IsVerified { get; set; }
    public string? Link { get; set; } = null!;
    public string? AddressDescription { get; set; } = null!;
    public string? Address { get; set; } = null!;
    public ServiceLocation ServiceLocation { get; set; }
    public List<ServiceCategoryModel>? ServiceCategories { get; set; }
    public List<TimeReservationModel>? TimeReservations { get; set; }
    public bool IsAvailable { get; set; }
    public Specialist Specialist { get; set; } = null!;
}