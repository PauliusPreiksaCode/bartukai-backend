using MediatR;

namespace portal_backend.Mediator.Commands;

public class ApproveServiceCommand : IRequest
{
    public int ServiceId { get; set; }
}