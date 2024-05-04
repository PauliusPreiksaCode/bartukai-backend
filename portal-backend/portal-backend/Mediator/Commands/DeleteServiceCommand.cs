using MediatR;

namespace portal_backend.Mediator.Commands;

public class DeleteServiceCommand : IRequest<bool>
{
    public int UserId { get; set; }
    public int ServiceId { get; set; }
}