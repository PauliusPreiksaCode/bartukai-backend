using MediatR;
using Microsoft.EntityFrameworkCore;
using portal_backend.Enums;
using portal_backend.Mediator.Commands;

namespace portal_backend.Mediator.Handlers;

public class EditOrderCommandHandler : IRequestHandler<EditOrderCommand>
{
    private readonly VcvsContext _vcvsContext;

    public EditOrderCommandHandler(VcvsContext vcvsContext)
    {
        _vcvsContext = vcvsContext;
    }

    public async Task Handle(EditOrderCommand request, CancellationToken cancellationToken)
    {
        var order = _vcvsContext.Order
            .Include(x => x.FullOrder)
            .Include(x => x.Customer)
            .Include(x => x.Customer.User)
            .Include(x => x.FullOrder.Service)
            .FirstOrDefault(x => x.Id == request.OrderId);

        if (order is null)
        {
            throw new Exception("Order doesn't exist");
        }

        if (order.Customer.User.Id != request.UserId)
        {
            throw new Exception("It's not user's order");
        }

        if (order.OrderStatus == OrderStatus.Cancelled)
        {
            throw new Exception("Order is canceled");
        }

        if (order.FullOrder.DateFrom < DateTime.Now)
        {
            throw new Exception("Order happened in the past");
        }

        var newReservation = _vcvsContext.FullOrder
            .Include(x => x.Order)
            .Include(x => x.Order!.Customer)
            .Include(x => x.Order!.Customer.User)
            .Include(x => x.Service)
            .FirstOrDefault(x => x.Id == request.NewReservationId);

        if (newReservation is null)
        {
            throw new Exception("Reservation time doesn't exist");
        }
        
        if (newReservation.DateFrom < DateTime.Now)
        {
            throw new Exception("Reservation happened in the past");
        }

        if (order.FullOrder.Service.Id != newReservation.Service.Id)
        {
            throw new Exception("Can't change to other service");
        }

        if (newReservation.Order is not null)
        {
            if (newReservation.Order.OrderStatus == OrderStatus.Created && newReservation.Order.Customer.User.Id != request.UserId)
            {
                throw new Exception("Reservation already occupied");
            }
        }

        var oldReservation = order.FullOrder;
        oldReservation.Order = null;
        
        order.Date = DateTime.Now;
        order.PaymentType = request.PaymentType;

        newReservation.Order = order;

        await _vcvsContext.SaveChangesAsync(cancellationToken);
    }
}