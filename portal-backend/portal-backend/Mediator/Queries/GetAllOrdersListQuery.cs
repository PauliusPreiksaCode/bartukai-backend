using MediatR;
using portal_backend.Models;

namespace portal_backend.Mediator.Queries;

public class GetAllOrdersListQuery : IRequest<List<OrderModel>>
{
    
}