using MediatR;
using portal_backend.Models;

namespace portal_backend.Mediator.Queries;

public class GetAllRoomsQuery : IRequest<List<RoomModel>>
{
    
}