using MediatR;

namespace portal_backend.Mediator.Commands;

public class DeleteUserCommand : IRequest
{
    public int Id { get; set; }
}