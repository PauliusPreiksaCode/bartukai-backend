using MediatR;
using portal_backend.Mediator.Queries;
using portal_backend.Models;

namespace portal_backend.Mediator.Handlers;

public class GetRoomOccupancyQueryHandler : IRequestHandler<GetRoomOccupancyQuery, List<TimeReservationModel>>
{
    private readonly VcvsContext _vcvsContext;

    public GetRoomOccupancyQueryHandler(VcvsContext vcvsContext)
    {
        _vcvsContext = vcvsContext;
    }

    public async Task<List<TimeReservationModel>> Handle(GetRoomOccupancyQuery request,
        CancellationToken cancellationToken)
    {
        var data = _vcvsContext.FullOrder
            .Where(y => y.OrderId != null)
            .Where(y => y.Room != null && y.Room.Id == request.RoomId)
            .Where(y => y.DateTo > DateTime.Now);

        var result = data
            .Select(y => new TimeReservationModel()
            {
                Id = y.Id,
                DateFrom = y.DateFrom,
                DateTo = y.DateTo,
                RoomId = y.Room != null ? y.Room.Id : null,
                EquipmentIds = y.Equipment != null ? y.Equipment.Select(z => z.Id).ToList() : null,
                RoomName = y.Room != null ? y.Room.Name : ""
            })
            .OrderBy(y => y.DateFrom)
            .ToList();

        return result;
    }
}