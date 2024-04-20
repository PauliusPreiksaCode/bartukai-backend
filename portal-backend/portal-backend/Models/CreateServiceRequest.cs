using portal_backend.Enums;

namespace portal_backend.Models;

public class CreateServiceRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; } = null!;
    public decimal Price { get; set; }
    public string? Link { get; set; } = null!;
    public string? AddressDescription { get; set; } = null!;
    public string? Address { get; set; } = null!;
    public ServiceLocation ServiceLocation { get; set; }
    public bool IsAvailable { get; set; }
    public List<int> ServiceCategories { get; set; } = null!;
    public List<TimeReservationModel> TimeReservations { get; set; }
}