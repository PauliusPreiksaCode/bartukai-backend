using MediatR;
using portal_backend.Mediator.Commands;

namespace portal_backend.Mediator.Handlers;

public class EditRoomCommandHandler : IRequestHandler<EditRoomCommand>
{
    private readonly VcvsContext _vcvsContext;
    
    public EditRoomCommandHandler(VcvsContext vcvsContext)
    {
        _vcvsContext = vcvsContext;
    } 
    
    public async Task Handle(EditRoomCommand request, CancellationToken cancellationToken)
    {
        var existingRoom = _vcvsContext.Room.FirstOrDefault(x => x.Id == request.Id);

        if (existingRoom == null)
        {
            throw new Exception("Room doesn't exist");
        }
        
        var otherRoom = _vcvsContext.Room.FirstOrDefault(x => 
            string.Equals(x.Name.ToLower(), request.Name.ToLower()) && x.Id != request.Id);

        if (otherRoom != null)
        {
            throw new Exception("Room name is already used by other room");
        }

        existingRoom.Name = request.Name;
        existingRoom.Accommodates = request.Accommodates;
        existingRoom.Description = request.Description;
        existingRoom.Floor = request.Floor;
        existingRoom.IsAvailable = request.IsAvailable;
        existingRoom.Type = request.Type;
        
        await _vcvsContext.SaveChangesAsync(cancellationToken);
    }
}