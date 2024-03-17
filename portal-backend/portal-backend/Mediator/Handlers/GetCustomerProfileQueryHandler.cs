using MediatR;
using portal_backend.Mediator.Queries;
using portal_backend.Models;

namespace portal_backend.Mediator.Handlers;

public class GetCustomerProfileQueryHandler : IRequestHandler<GetCustomerProfileQuery, CustomerProfileModel>
{
    private readonly VcvsContext _vcvsContext;
    
    public GetCustomerProfileQueryHandler(VcvsContext vcvsContext)
    {
        _vcvsContext = vcvsContext;
    }
    
    public async Task<CustomerProfileModel> Handle(GetCustomerProfileQuery request, CancellationToken cancellationToken)
    {
        var user = _vcvsContext.User.FirstOrDefault(x => x.Id == request.UserId);

        if (user is null)
        {
            throw new Exception("User doesn't exist");
        }

        var customer = _vcvsContext.Customer.FirstOrDefault(x => x.User.Id == request.UserId);

        if (customer is null)
        {
            throw new Exception("User is not customer");
        }

        return new CustomerProfileModel()
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName,
            Email = user.Email,
            City = user.City,
            BirthDate = user.BirthDate,
            Gender = user.Gender,
            PhoneNumber = user.PhoneNumber,
            CreationTime = user.CreationTime,
            LastOrderedService = customer.LastOrderedService,
            LoyaltyGroup = customer.LoyaltyGroup,
        };
    }
}