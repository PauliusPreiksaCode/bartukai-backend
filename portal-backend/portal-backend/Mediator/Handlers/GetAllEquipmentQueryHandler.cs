using MediatR;
using portal_backend.Mediator.Queries;
using portal_backend.Models;

namespace portal_backend.Mediator.Handlers;

public class GetAllEquipmentQueryHandler : IRequestHandler<GetAllEquipmentQuery, List<EquipmentModel>>
{
    private readonly VcvsContext _vcvsContext;
    
    public GetAllEquipmentQueryHandler(VcvsContext vcvsContext)
    {
        _vcvsContext = vcvsContext;
    }

    public async Task<List<EquipmentModel>> Handle(GetAllEquipmentQuery request, CancellationToken cancellationToken)
    {
        var equipmentDb = _vcvsContext.Equipment.ToList();

        var result = equipmentDb.Select(x => new EquipmentModel()
        {
            Id = x.Id,
            Name = x.Name,
            InventoryNumber = x.InventoryNumber,
            Condition = x.Condition,
            Type = x.Type,
            IsAvailable = x.IsAvailable
        })
            .OrderBy(x => x.Name)
            .ToList();

        return result;
    }
}