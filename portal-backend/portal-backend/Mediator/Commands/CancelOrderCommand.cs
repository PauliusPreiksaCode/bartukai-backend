using MediatR;

namespace portal_backend.Mediator.Commands;

public class CancelOrderCommand : IRequest
{
    public int UserId { get; set; }
    public int OrderId { get; set; }
}