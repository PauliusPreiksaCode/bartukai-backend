using portal_backend.Enums;

namespace portal_backend.Models;

public class TimeReservationModel
{
    public int? Id { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int? RoomId { get; set; }
    public List<int>? EquipmentIds { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerLastName { get; set; }
    public string? CustomerEmail { get; set; }
    public DateTime? CustomerOrderDate { get; set; }
    public PaymentType? PaymentType { get; set; }
    public OrderStatus? OrderStatus { get; set; }
    public int? OrderNumber { get; set; }
    public string? RoomName { get; set; }
    public string? RoomDescription { get; set; }
    public List<string>? EquipmentNames { get; set; }
}