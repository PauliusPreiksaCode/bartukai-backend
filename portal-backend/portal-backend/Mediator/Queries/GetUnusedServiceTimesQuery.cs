using MediatR;
using portal_backend.Models;

namespace portal_backend.Mediator.Queries;

public class GetUnusedServiceTimesQuery : IRequest<List<TimeReservationModel>>
{
    public int ServiceId { get; set; }
}