using MediatR;
using portal_backend.Enums;
using portal_backend.Mediator.Queries;

namespace portal_backend.Mediator.Handlers;

public class GetUserAccountTypeQueryHandler : IRequestHandler<GetUserAccountTypeQuery, AccountType>
{
    private readonly VcvsContext _vcvsContext;
    
    public GetUserAccountTypeQueryHandler(VcvsContext vcvsContext)
    {
        _vcvsContext = vcvsContext;
    }

    public async Task<AccountType> Handle(GetUserAccountTypeQuery request, CancellationToken cancellationToken)
    {
        var existingCustomer = _vcvsContext.Customer.FirstOrDefault(customer => customer.User.Id.Equals(request.UserId));
        if (existingCustomer is not null)
        {
            return AccountType.Customer;
        }
        
        var existingSpecialist = _vcvsContext.Specialist.FirstOrDefault(specialist => specialist.User.Id.Equals(request.UserId));
        if (existingSpecialist is not null)
        {
            return AccountType.Specialist;
        }
        
        var existingAdmin = _vcvsContext.Admin.FirstOrDefault(admin => admin.User.Id.Equals(request.UserId));
        if (existingAdmin is not null)
        {
            return AccountType.Admin;
        }

        throw new Exception("User doesn't exist");
    }
}