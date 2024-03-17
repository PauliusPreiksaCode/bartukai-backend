using MediatR;

namespace portal_backend.Mediator.Commands;

public class EditEquipmentCommand : IRequest
{
    public int Id { get; set; }
    public int? InventoryNumber { get; set; }
    public string Name { get; set; } = null!;
    public string? Condition { get; set; }
    public string? Type { get; set; }
    public bool IsAvailable { get; set; }
}