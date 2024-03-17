using MediatR;

namespace portal_backend.Mediator.Commands;

public class LoginUserCommand : IRequest<int?>
{
    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;
}