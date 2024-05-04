using MediatR;
using portal_backend.Models;

namespace portal_backend.Mediator.Queries;

public class GetRoomOccupancyQuery : IRequest<List<TimeReservationModel>>
{
    public int RoomId { get; set; }
}