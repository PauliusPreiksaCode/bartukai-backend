using MediatR;
using portal_backend.Mediator.Commands;

namespace portal_backend.Mediator.Handlers;

public class SetLastLoginDateCommandHandler : IRequestHandler<SetLastLoginDateCommand, Unit>
{
    private readonly VcvsContext _vcvsContext;
    public SetLastLoginDateCommandHandler(VcvsContext vcvsContext)
    {
        _vcvsContext = vcvsContext;
    }

    public async Task<Unit> Handle(SetLastLoginDateCommand request, CancellationToken cancellationToken)
    {
        var existingCustomer = _vcvsContext.Customer.FirstOrDefault(customer => customer.User.Id.Equals(request.UserId));
        if (existingCustomer is not null)
        {
            existingCustomer.LastLogin = request.LastLoginDate;
            await _vcvsContext.SaveChangesAsync(cancellationToken);
        }
        
        return Unit.Value;
    }
}