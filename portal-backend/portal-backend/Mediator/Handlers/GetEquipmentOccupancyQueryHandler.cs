using MediatR;
using Microsoft.EntityFrameworkCore;
using portal_backend.Mediator.Queries;
using portal_backend.Models;

namespace portal_backend.Mediator.Handlers;

public class GetEquipmentOccupancyQueryHandler : IRequestHandler<GetEquipmentOccupancyQuery, List<TimeReservationModel>>
{
    private readonly VcvsContext _vcvsContext;

    public GetEquipmentOccupancyQueryHandler(VcvsContext vcvsContext)
    {
        _vcvsContext = vcvsContext;
    }

    public async Task<List<TimeReservationModel>> Handle(GetEquipmentOccupancyQuery request,
        CancellationToken cancellationToken)
    {
        var data = _vcvsContext.FullOrder
            .Include(y => y.Equipment)
            .Include(y => y.Room)
            .ToList();
            
            data = data
            .Where(y => y.OrderId != null)
            .Where(y => y.Equipment != null && y.Equipment.Any(x => x.Id == request.EquipmentId))
            .Where(y => y.DateTo > DateTime.Now)
            .ToList();

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