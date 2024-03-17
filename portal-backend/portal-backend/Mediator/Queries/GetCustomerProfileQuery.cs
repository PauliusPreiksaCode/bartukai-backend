using MediatR;
using portal_backend.Models;

namespace portal_backend.Mediator.Queries;

public class GetCustomerProfileQuery : IRequest<CustomerProfileModel>
{
    public int UserId { get; set; }
}