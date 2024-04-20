using portal_backend.Enums;

namespace portal_backend.Models;

public class OrderModel
{
    public int Id { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? RoomName { get; set; }
    public string? Address { get; set; }
    public string? AddressDescription { get; set; }
    public string? Link { get; set; }
    public ServiceLocation? ServiceLocation { get; set; }
    public string CustomerFirstName { get; set; }
    public string CustomerLastName { get; set; }
    public string CustomerEmail { get; set; }
    public DateTime CustomerOrderDate { get; set; }
    public PaymentType PaymentType { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public int? OrderNumber { get; set; }
    public string? SpecialistFirstName { get; set; }
    public string? SpecialistLastName { get; set; }
    public string? ServiceName { get; set; }
    public int? ServiceId { get; set; }
    public int? ReservationId { get; set; }
}