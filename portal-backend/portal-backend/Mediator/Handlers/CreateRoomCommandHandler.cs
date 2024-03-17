using MediatR;
using portal_backend.Entities;
using portal_backend.Mediator.Commands;

namespace portal_backend.Mediator.Handlers;

public class CreateRoomCommandHandler : IRequestHandler<CreateRoomCommand>
{
    private readonly VcvsContext _vcvsContext;
    
    public CreateRoomCommandHandler(VcvsContext vcvsContext)
    {
        _vcvsContext = vcvsContext;
    } 
    
    public async Task Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
        var existingRoom = _vcvsContext.Room.FirstOrDefault(x => 
            string.Equals(x.Name.ToLower(), request.Name.ToLower()));

        if (existingRoom != null)
        {
            throw new Exception("Room already exists");
        }

        var room = new Room
        {
            Name = request.Name,
            Accommodates = request.Accommodates,
            Description = request.Description,
            Floor = request.Floor,
            FullOrder = new List<FullOrder>(),
            IsAvailable = request.IsAvailable,
            Type = request.Type
        };

        _vcvsContext.Room.Add(room);
        await _vcvsContext.SaveChangesAsync(cancellationToken);
    }
}