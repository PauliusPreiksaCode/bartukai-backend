using MediatR;
using portal_backend.Models;

namespace portal_backend.Mediator.Queries;

public class GetAllCustomerOrdersListQuery : IRequest<List<OrderModel>>
{
    public int UserId { get; set; }
}