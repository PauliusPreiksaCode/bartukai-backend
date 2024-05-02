using MediatR;
using Microsoft.EntityFrameworkCore;
using portal_backend.Enums;
using portal_backend.Mediator.Commands;

namespace portal_backend.Mediator.Handlers;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand>
{
    public VcvsContext _vcvsContext;

    public CancelOrderCommandHandler(VcvsContext vcvsContext)
    {
        _vcvsContext = vcvsContext;
    }

    public async Task Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = _vcvsContext.Order
            .Include(x => x.Customer)
            .Include(x => x.Customer.User)
            .Include(x => x.FullOrder)
            .FirstOrDefault(x => x.Id == request.OrderId);

        if (order is null)
        {
            throw new Exception("Order doesn't exist");
        }

        if (order.Customer.User.Id != request.UserId)
        {
            throw new Exception("It's not user's order");
        }
        
        if (order.FullOrder.DateFrom < DateTime.Now)
        {
            throw new Exception("Time in the past");
        }
        
        if (order.OrderStatus == OrderStatus.Cancelled)
        {
            throw new Exception("Order already canceled");
        }

        var reservation = _vcvsContext.FullOrder.FirstOrDefault(x => x.OrderId == order.Id);

        if (reservation is null)
        {
            throw new Exception("Reservation doesn't exist");
        }
        
        order.OrderStatus = OrderStatus.Cancelled;
        reservation.Order = null;
        
        await _vcvsContext.SaveChangesAsync(cancellationToken);
    }
}