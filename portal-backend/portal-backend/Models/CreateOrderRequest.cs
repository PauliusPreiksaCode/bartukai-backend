using portal_backend.Enums;

namespace portal_backend.Models;

public class CreateOrderRequest
{
    public int TimeReservationId { get; set; }
    public  PaymentType PaymentType { get; set; }
}