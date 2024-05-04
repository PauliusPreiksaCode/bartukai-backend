using MediatR;
using portal_backend.Models;

namespace portal_backend.Mediator.Queries;

public class GetEquipmentOccupancyQuery : IRequest<List<TimeReservationModel>>
{
    public int EquipmentId { get; set; }
}