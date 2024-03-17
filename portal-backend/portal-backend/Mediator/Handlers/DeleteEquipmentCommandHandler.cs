using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using portal_backend.Entities;
using portal_backend.Mediator.Commands;

namespace portal_backend.Mediator.Handlers;

public class DeleteEquipmentCommandHandler : IRequestHandler<DeleteEquipmentCommand, bool>
{
    private readonly VcvsContext _vcvsContext;

    public DeleteEquipmentCommandHandler(VcvsContext vcvsContext)
    {
        _vcvsContext = vcvsContext;
    }

    public async Task<bool> Handle(DeleteEquipmentCommand request, CancellationToken cancellationToken)
    {
        var equipment = _vcvsContext.Equipment
            .FirstOrDefault(
                x => x.Id == request.EquipmentId);

        if (equipment is null)
        {
            throw new Exception("Equipment doesn't exist");
        }

        equipment.IsAvailable = false;

        var orderedTimes = _vcvsContext.FullOrder
            .Include(x => x.Equipment)
            .Where(x => x.DateFrom > DateTime.Now)
            .AsEnumerable()
            .Where(x => (x.Equipment ?? new List<Equipment>()).Any(y => y.Id == equipment.Id));
        
        if (orderedTimes.IsNullOrEmpty())
        {
            DeleteEquipment(equipment);
            await _vcvsContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        
        await _vcvsContext.SaveChangesAsync(cancellationToken);
        return false;
    }

    private void DeleteEquipment(Equipment equipment)
    {
        _vcvsContext.Equipment.Remove(equipment);
    }
}