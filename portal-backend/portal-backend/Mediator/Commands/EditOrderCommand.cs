using MediatR;
using portal_backend.Enums;

namespace portal_backend.Mediator.Commands;

public class EditOrderCommand : IRequest
{
    public int UserId { get; set; }
    public int OrderId { get; set; }
    public int NewReservationId { get; set; }
    public PaymentType PaymentType { get; set; }
}