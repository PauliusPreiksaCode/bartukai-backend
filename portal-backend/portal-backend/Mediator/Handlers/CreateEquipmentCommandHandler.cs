using MediatR;
using portal_backend.Entities;
using portal_backend.Mediator.Commands;

namespace portal_backend.Mediator.Handlers;

public class CreateEquipmentCommandHandler : IRequestHandler<CreateEquipmentCommand>
{
    private readonly VcvsContext _vcvsContext;
    
    public CreateEquipmentCommandHandler(VcvsContext vcvsContext)
    {
        _vcvsContext = vcvsContext;
    }

    public async Task Handle(CreateEquipmentCommand request, CancellationToken cancellationToken)
    {
        var existingEquipment = _vcvsContext.Equipment.FirstOrDefault(x => 
            string.Equals(x.InventoryNumber, request.InventoryNumber));

        if (existingEquipment != null)
        {
            throw new Exception("Equipment already exists");
        }

        var equipment = new Equipment
        {
            Name = request.Name,
            InventoryNumber = request.InventoryNumber,
            Condition = request.Condition,
            Type = request.Type,
            FullOrders = new List<FullOrder>(),
            IsAvailable = request.IsAvailable
        };
        

        _vcvsContext.Equipment.Add(equipment);
        await _vcvsContext.SaveChangesAsync(cancellationToken);
    }
}