using MediatR;
using portal_backend.Enums;
using portal_backend.Models;

namespace portal_backend.Mediator.Commands;

public class EditServiceCommand : IRequest
{
    public int UserId { get; set; }
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? Link { get; set; }
    public string? AddressDescription { get; set; }
    public string? Address { get; set; }
    public bool IsAvailable { get; set; }
    public List<int> ServiceCategories { get; set; }
    public List<TimeReservationModel> TimeReservations { get; set; }
}