using MediatR;
using portal_backend.Helpers;
using portal_backend.Mediator.Commands;

namespace portal_backend.Mediator.Handlers;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, int?>
{
    private readonly VcvsContext _vcvsContext;
    private readonly IConfiguration _config;
    
    public LoginUserCommandHandler(VcvsContext vcvsContext, IConfiguration config)
    {
        _vcvsContext = vcvsContext;
        _config = config;
    }

    public Task<int?> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser =
            _vcvsContext.User.FirstOrDefault(
                user => user.Email == request.UserName || user.UserName == request.UserName);
        if (existingUser == null) throw new Exception("User not found");

        var passwordHash =
            AuthorizationHelpers.ComputeSha256Hash(request.Password + _config["StaticSalt"] + existingUser.Salt);

        if (existingUser.Password.Equals(passwordHash))
        {
            return Task.FromResult((int?)existingUser.Id);
        }

        throw new Exception("User not found");
    }
}