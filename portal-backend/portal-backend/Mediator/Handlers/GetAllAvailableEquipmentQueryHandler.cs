using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using portal_backend.Entities;
using portal_backend.Mediator.Queries;
using portal_backend.Models;

namespace portal_backend.Mediator.Handlers;

public class GetAllAvailableEquipmentQueryHandler : IRequestHandler<GetAllAvailableEquipmentQuery, List<EquipmentModel>>
{
    private readonly VcvsContext _vcvsContext;
    
    public GetAllAvailableEquipmentQueryHandler(VcvsContext vcvsContext)
    {
        _vcvsContext = vcvsContext;
    }
    
    public async Task<List<EquipmentModel>> Handle(GetAllAvailableEquipmentQuery request, CancellationToken cancellationToken)
    {
        if (request.DateFrom >= request.DateTo || request.DateFrom < DateTime.Now)
        {
            return new List<EquipmentModel>();
        }
        
        var generallyAvailableEquipmentDb = _vcvsContext.Equipment
            .Where(x => x.IsAvailable)
            .Include(x => x.FullOrders);

        var availableEquipment = new List<Equipment>();

        foreach (var equipment in generallyAvailableEquipmentDb)
        {
            var list = (equipment.FullOrders ?? new List<FullOrder>())
                .Where(y => !(request.DateTo <= y.DateFrom || y.DateTo <= request.DateFrom));

            if (list.IsNullOrEmpty())
            {
                availableEquipment.Add(equipment);
            }
        }

        var result = availableEquipment.Select(x => new EquipmentModel()
            {
                Id = x.Id,
                InventoryNumber = x.InventoryNumber,
                Condition = x.Condition,
                Name = x.Name,
                Type = x.Type,
                IsAvailable = x.IsAvailable
            
            })
            .OrderBy(x => x.Name)
            .ToList();

        return result;
    }
}