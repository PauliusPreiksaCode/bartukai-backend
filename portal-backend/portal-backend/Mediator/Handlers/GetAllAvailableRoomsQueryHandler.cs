using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using portal_backend.Entities;
using portal_backend.Mediator.Queries;
using portal_backend.Models;

namespace portal_backend.Mediator.Handlers;

public class GetAllAvailableRoomsQueryHandler : IRequestHandler<GetAllAvailableRoomsQuery, List<RoomModel>>
{
    private readonly VcvsContext _vcvsContext;
    
    public GetAllAvailableRoomsQueryHandler(VcvsContext vcvsContext)
    {
        _vcvsContext = vcvsContext;
    }
    
    public async Task<List<RoomModel>> Handle(GetAllAvailableRoomsQuery request, CancellationToken cancellationToken)
    {
        if (request.DateFrom >= request.DateTo || request.DateFrom < DateTime.Now)
        {
            return new List<RoomModel>();
        }
        
        var generallyAvailableRoomsDb = _vcvsContext.Room
            .Where(x => x.IsAvailable)
            .Include(x => x.FullOrder);

        var availableRooms = new List<Room>();

        foreach (var room in generallyAvailableRoomsDb)
        {
            var list = (room.FullOrder ?? new List<FullOrder>())
                .Where(y => !(request.DateTo <= y.DateFrom || y.DateTo <= request.DateFrom));

            if (list.IsNullOrEmpty())
            {
                availableRooms.Add(room);
            }
        }

        var result = availableRooms.Select(x => new RoomModel()
            {
                Id = x.Id,
                Name = x.Name,
                Type = x.Type,
                Floor = x.Floor,
                Accommodates = x.Accommodates,
                Description = x.Description,
                IsAvailable = x.IsAvailable
            
            })
            .OrderBy(x => x.Name)
            .ToList();

        return result;
    }
}