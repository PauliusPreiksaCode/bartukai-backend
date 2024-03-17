using MediatR;
using portal_backend.Models;

namespace portal_backend.Mediator.Queries;

public class GetAllEquipmentQuery : IRequest<List<EquipmentModel>>
{
    
}