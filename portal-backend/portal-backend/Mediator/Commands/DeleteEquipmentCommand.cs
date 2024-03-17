using MediatR;

namespace portal_backend.Mediator.Commands;

public class DeleteEquipmentCommand : IRequest<bool>
{
    public int EquipmentId { get; set; }
}