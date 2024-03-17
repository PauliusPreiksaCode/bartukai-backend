using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using portal_backend.Entities;
using portal_backend.Mediator.Commands;

namespace portal_backend.Mediator.Handlers;

public class DeleteRoomCommandHandler : IRequestHandler<DeleteRoomCommand, bool>
{
    private VcvsContext _vcvsContext;

    public DeleteRoomCommandHandler(VcvsContext vcvsContext)
    {
        _vcvsContext = vcvsContext;
    }

    public async Task<bool> Handle(DeleteRoomCommand request, CancellationToken cancellationToken)
    {
        var room = _vcvsContext.Room
            .FirstOrDefault(
                x => x.Id == request.RoomId);

        if (room is null)
        {
            throw new Exception("Room doesn't exist");
        }

        room.IsAvailable = false;

        var orderedTimes =  _vcvsContext.FullOrder
            .Where(x => x.DateFrom > DateTime.Now
                        && (x.Room != null ? x.Room.Id : -1) == room.Id).ToList();

        if (orderedTimes.IsNullOrEmpty())
        {
            DeleteRoom(room);
            await _vcvsContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        await _vcvsContext.SaveChangesAsync(cancellationToken);
        return false;
    }

    private void DeleteRoom(Room room)
    {
        foreach (var reservation in _vcvsContext.FullOrder.Where(x => (x.Room != null ? x.Room.Id : 1) == room.Id))
        {
            if (reservation.OrderId is null)
            {
                reservation.Room = null;
            }
        }
        
        _vcvsContext.Room.Remove(room);
    }
}