using MediatR;
using Microsoft.EntityFrameworkCore;
using portal_backend.Entities;
using portal_backend.Enums;
using portal_backend.Mediator.Commands;

namespace portal_backend.Mediator.Handlers;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, int>
{
    private readonly VcvsContext _vcvsContext;
    
    public CreateOrderCommandHandler(VcvsContext vcvsContext)
    {
        _vcvsContext = vcvsContext;
    }
    
    public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var existingTimeReservation = _vcvsContext.FullOrder
            .Include(x => x.Service)
            .FirstOrDefault(x => x.Id == request.TimeReservationId);

        if (existingTimeReservation is null)
        {
            throw new Exception("Time reservation doesn't exist");
        }

        if (existingTimeReservation.OrderId is not null)
        {
            throw new Exception("Time already reserved");
        }

        if (existingTimeReservation.DateFrom < DateTime.Now)
        {
            throw new Exception("Time in the past");
        }

        if (!existingTimeReservation.Service.IsAvailable || !existingTimeReservation.Service.IsVerified)
        {
            throw new Exception("Service is unavailable");
        }

        var customer = _vcvsContext.Customer.FirstOrDefault(x => x.User.Id == request.UserId);

        if (customer is null)
        {
            throw new Exception("Customer doesn't exist");
        }

        var order = new Order()
        {
            OrderNumber = Math.Abs((int)DateTime.Now.Ticks),
            Date = DateTime.Now,
            PaymentType = request.PaymentType,
            OrderStatus = OrderStatus.Created,
            Customer = customer,
            FullOrder = existingTimeReservation
        };
        
        _vcvsContext.Order.Add(order);

        customer.LastOrderedService = existingTimeReservation.Service.Name;
        IncreaseLoyaltyGroup(customer);

        await _vcvsContext.SaveChangesAsync(cancellationToken);

        return order.Id;
    }

    private void IncreaseLoyaltyGroup(Customer customer)
    {
        var current = customer.LoyaltyGroup;

        switch (current)
        {
            case LoyaltyGroup.Bartukas:
                customer.LoyaltyGroup = LoyaltyGroup.Sidabriukas;
                break;
            case LoyaltyGroup.Sidabriukas:
                customer.LoyaltyGroup = LoyaltyGroup.Auksiukas;
                break;
            case LoyaltyGroup.Auksiukas:
                customer.LoyaltyGroup = LoyaltyGroup.Deimantukas;
                break;
            case LoyaltyGroup.Deimantukas:
                break;
            case null:
                customer.LoyaltyGroup = LoyaltyGroup.Bartukas;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}