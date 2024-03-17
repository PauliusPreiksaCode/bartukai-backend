using MediatR;
using portal_backend.Mediator.Queries;
using portal_backend.Models;

namespace portal_backend.Mediator.Handlers;

public class GetAllRoomsQueryHandler : IRequestHandler<GetAllRoomsQuery, List<RoomModel>>
{
    private readonly VcvsContext _vcvsContext;
    
    public GetAllRoomsQueryHandler(VcvsContext vcvsContext)
    {
        _vcvsContext = vcvsContext;
    }

    public async Task<List<RoomModel>> Handle(GetAllRoomsQuery request, CancellationToken cancellationToken)
    {
        var roomsDb = _vcvsContext.Room.ToList();

        var result = roomsDb.Select(x => new RoomModel()
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