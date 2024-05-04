using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using portal_backend.Entities;
using portal_backend.Mediator.Commands;

namespace portal_backend.Mediator.Handlers;

public class DeleteServiceCommandHandler : IRequestHandler<DeleteServiceCommand, bool>
{
    private readonly VcvsContext _vcvsContext;

    public DeleteServiceCommandHandler(VcvsContext vcvsContext)
    {
        _vcvsContext = vcvsContext;
    }

    public async Task<bool> Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
    {
        var service = _vcvsContext.Service
            .Include(x => x.FullOrders)
            .FirstOrDefault(
                x => x.Id == request.ServiceId && x.Specialist.User.Id == request.UserId);

        if (service is null)
        {
            throw new Exception("Service doesn't exist");
        }

        var reservations = service.FullOrders.Where(x => x.OrderId != null && x.DateFrom > DateTime.Now).ToList();

        if (reservations.IsNullOrEmpty())
        {
            DeleteService(service);
            await _vcvsContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        RemoveFreeTime(service);
        await _vcvsContext.SaveChangesAsync(cancellationToken);
        return false;
    }

    private void RemoveFreeTime(Service service)
    {
        foreach (var reservation in service.FullOrders.Where(x => x.DateFrom > DateTime.Now))
        {
            if (reservation.OrderId is null)
            {
                service.FullOrders.Remove(reservation);
            }
        }
    }

    private void DeleteService(Service service)
    {
        _vcvsContext.Service.Remove(service);
    }
}