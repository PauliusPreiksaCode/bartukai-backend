using MediatR;
using portal_backend.Enums;

namespace portal_backend.Mediator.Commands;

public class CreateOrderCommand : IRequest<int>
{
    public int UserId { get; set; }
    public int TimeReservationId { get; set; }
    public  PaymentType PaymentType { get; set; }
}