using MediatR;

namespace portal_backend.Mediator.Commands;

public class SetLastLoginDateCommand : IRequest<Unit>
{
    public int UserId { get; set; }
    public DateTime LastLoginDate { get; set; }
}