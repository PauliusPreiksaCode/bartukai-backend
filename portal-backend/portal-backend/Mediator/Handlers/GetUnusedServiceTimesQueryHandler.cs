using MediatR;
using Microsoft.EntityFrameworkCore;
using portal_backend.Mediator.Queries;
using portal_backend.Models;

namespace portal_backend.Mediator.Handlers;

public class GetUnusedServiceTimesQueryHandler : IRequestHandler<GetUnusedServiceTimesQuery, List<TimeReservationModel>>
{
    private readonly VcvsContext _vcvsContext;

    public GetUnusedServiceTimesQueryHandler(VcvsContext vcvsContext)
    {
        _vcvsContext = vcvsContext;
    }

    public async Task<List<TimeReservationModel>> Handle(GetUnusedServiceTimesQuery request, CancellationToken cancellationToken)
    {
        var list = _vcvsContext.FullOrder
            .Include(x => x.Service)
            .Where(x => x.Service.Id == request.ServiceId && x.OrderId == null && x.DateFrom > DateTime.Now)
            .Select(x => new TimeReservationModel()
            {
                Id = x.Id,
                DateFrom = x.DateFrom,
                DateTo = x.DateTo,
                RoomId = x.Room != null ? x.Room.Id : null,
                RoomName = x.Room != null ? x.Room.Name : null
            })
            .OrderBy(x => x.DateFrom)
            .ToList();

        return list;
    }
}