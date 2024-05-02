using portal_backend.Enums;

namespace portal_backend.Models;

public class EditOrderRequest
{
    public int OrderId { get; set; }
    public int NewReservationId { get; set; }
    public PaymentType PaymentType { get; set; }
}