using MediatR;

namespace portal_backend.Mediator.Commands;

public class DeleteRoomCommand : IRequest<bool>
{
    public int RoomId { get; set; }
}