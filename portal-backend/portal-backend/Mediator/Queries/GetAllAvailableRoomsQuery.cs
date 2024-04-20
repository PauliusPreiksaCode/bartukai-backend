using MediatR;
using portal_backend.Models;

namespace portal_backend.Mediator.Queries;

public class GetAllAvailableRoomsQuery : IRequest<List<RoomModel>>
{
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
}