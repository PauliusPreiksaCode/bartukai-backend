using MediatR;

namespace portal_backend.Mediator.Commands;

public class EditRoomCommand : IRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
    public int Floor { get; set; }
    public int Accommodates { get; set; }
    public string Description { get; set; } = null!;
    public bool IsAvailable { get; set; }
}