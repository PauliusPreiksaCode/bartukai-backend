using MediatR;
using Microsoft.EntityFrameworkCore;
using portal_backend.Mediator.Queries;
using portal_backend.Models;

namespace portal_backend.Mediator.Handlers;

public class GetAllCustomerOrdersListQueryHandler : IRequestHandler<GetAllCustomerOrdersListQuery, List<OrderModel>>
{
    public VcvsContext _vcvsContext;

    public GetAllCustomerOrdersListQueryHandler(VcvsContext vcvsContext)
    {
        _vcvsContext = vcvsContext;
    }

    public async Task<List<OrderModel>> Handle(GetAllCustomerOrdersListQuery request, CancellationToken cancellationToken)
    {
        var result = _vcvsContext.Order
            .Include(x => x.FullOrder)
            .Where(x => x.Customer.User.Id == request.UserId)
            .Select(x => new OrderModel()
            {
                Id = x.Id,
                DateFrom = x.FullOrder.DateFrom,
                DateTo = x.FullOrder.DateTo,
                RoomName = x.FullOrder.Room != null ? x.FullOrder.Room.Name : null,
                Address = x.FullOrder.Service.Address,
                AddressDescription = x.FullOrder.Service.AddressDescription,
                Link = x.FullOrder.Service.Link,
                ServiceLocation = x.FullOrder.Service.ServiceLocation,
                CustomerFirstName = x.Customer.User.FirstName,
                CustomerLastName = x.Customer.User.LastName,
                CustomerEmail = x.Customer.User.Email,
                CustomerOrderDate = x.Date,
                PaymentType = x.PaymentType,
                OrderStatus = x.OrderStatus,
                OrderNumber = x.OrderNumber,
                SpecialistFirstName = x.FullOrder.Service.Specialist.User.FirstName,
                SpecialistLastName = x.FullOrder.Service.Specialist.User.LastName,
                ServiceName = x.FullOrder.Service.Name,
                ServiceId = x.FullOrder.Service.Id,
                ReservationId = x.FullOrder.Id
            })
            .OrderBy(x => x.DateFrom)
            .ToList();

        return result;
    }
}