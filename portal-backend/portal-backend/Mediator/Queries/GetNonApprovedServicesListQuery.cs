using MediatR;
using portal_backend.Models;

namespace portal_backend.Mediator.Queries;

public class GetNonApprovedServicesListQuery : IRequest<List<ServiceModel>>
{
    
}