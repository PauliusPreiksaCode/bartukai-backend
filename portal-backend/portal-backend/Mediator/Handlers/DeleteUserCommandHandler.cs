using System.Security.Cryptography;
using MediatR;
using portal_backend.Mediator.Commands;

namespace portal_backend.Mediator.Handlers;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly VcvsContext _vcvsContext;
    
    public DeleteUserCommandHandler(VcvsContext vcvsContext)
    {
        _vcvsContext = vcvsContext;
    }

    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = _vcvsContext.User.FirstOrDefault(user => user.Id == request.Id);

        if (user is null)
            throw new Exception("User not found");

        user.UserName = user.UserName + "-deleted-" + Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        user.Email = user.UserName;
        user.Password = new Guid().ToString();
        
        await _vcvsContext.SaveChangesAsync(cancellationToken);
    }
}