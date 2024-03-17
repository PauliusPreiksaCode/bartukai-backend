using MediatR;

namespace portal_backend.Mediator.Commands;

public class SendRegisterEmailCommand : IRequest
{
    public int UserId { get; set; }
}