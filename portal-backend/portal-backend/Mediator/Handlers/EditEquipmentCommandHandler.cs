using MediatR;
using portal_backend.Mediator.Commands;

namespace portal_backend.Mediator.Handlers;

public class EditEquipmentCommandHandler : IRequestHandler<EditEquipmentCommand>
{
    private readonly VcvsContext _vcvsContext;
    
    public EditEquipmentCommandHandler(VcvsContext vcvsContext)
    {
        _vcvsContext = vcvsContext;
    } 
    
    public async Task Handle(EditEquipmentCommand request, CancellationToken cancellationToken)
    {
        var existingEquipment = _vcvsContext.Equipment.FirstOrDefault(x => x.Id == request.Id);

        if (existingEquipment == null)
        {
            throw new Exception("Equipment doesn't exist");
        }
        
        var otherEquipment = _vcvsContext.Equipment.FirstOrDefault(x => 
            Equals(x.InventoryNumber, request.InventoryNumber) && x.Id != request.Id);

        if (otherEquipment != null)
        {
            throw new Exception("Equipment inventory number is already used by other equipment");
        }

        existingEquipment.Name = request.Name;
        existingEquipment.InventoryNumber = request.InventoryNumber;
        existingEquipment.Condition = request.Condition;
        existingEquipment.Type = request.Type;
        existingEquipment.IsAvailable = request.IsAvailable;
        
        await _vcvsContext.SaveChangesAsync(cancellationToken);
    }
}