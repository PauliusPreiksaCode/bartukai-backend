using MediatR;
using Microsoft.EntityFrameworkCore;
using portal_backend.Entities;
using portal_backend.Mediator.Queries;
using portal_backend.Models;
using portal_backend.Services;

namespace portal_backend.Mediator.Handlers;

public class GetSpecialistServicesQueryHandler: IRequestHandler<GetSpecialistServicesQuery, List<ServiceModel>>
{
    private readonly VcvsContext _vcvsContext;
    private readonly ServiceService _serviceService;
    
    public GetSpecialistServicesQueryHandler(VcvsContext vcvsContext, ServiceService serviceService)
    {
        _vcvsContext = vcvsContext;
        _serviceService = serviceService;
    }

    public async Task<List<ServiceModel>> Handle(GetSpecialistServicesQuery request, CancellationToken cancellationToken)
    {
        var result = _vcvsContext.Service
            .Include(service => service.Specialist)
            .Include(service => service.Specialist.User)
            .Include(service => service.ServiceCategories)
            .Select(x => new ServiceModel()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Price = x.Price,
                IsVerified = x.IsVerified,
                Link = x.Link,
                AddressDescription = x.AddressDescription,
                Address = x.Address,
                ServiceLocation = x.ServiceLocation,
                IsAvailable = x.IsAvailable,
                Specialist = new Specialist
                {
                    Id = x.Specialist.Id,
                    Photo = x.Specialist.Photo,
                    Description = x.Specialist.Description,
                    Experience = x.Specialist.Experience,
                    Education = x.Specialist.Education,
                    AgreementId = x.Specialist.AgreementId,
                    User = x.Specialist.User
                },
                ServiceCategories = x.ServiceCategories.Select(y => new ServiceCategoryModel()
                {
                    Id = y.Id,
                    Name = y.Name,
                    Description = y.Description
                })
                    .OrderBy(y => y.Name)
                    .ToList(),
                TimeReservations = x.FullOrders.Select(y => new TimeReservationModel()
                {
                    Id = y.Id,
                    DateFrom = y.DateFrom,
                    DateTo = y.DateTo,
                    RoomId = y.Room != null ? y.Room.Id : null,
                    EquipmentIds = y.Equipment != null ? y.Equipment.Select(z => z.Id).ToList() : null,
                    CustomerName = y.Order != null ? y.Order.Customer.User.FirstName : null,
                    CustomerLastName = y.Order != null ? y.Order.Customer.User.LastName : null,
                    CustomerEmail = y.Order != null ? y.Order.Customer.User.Email : null,
                    CustomerOrderDate = y.Order != null ? y.Order.Date : null,
                    PaymentType = y.Order != null ? y.Order.PaymentType : null,
                    OrderStatus = y.Order != null ? y.Order.OrderStatus : null,
                    OrderNumber = y.Order != null ? y.Order.OrderNumber : null,
                    RoomName = y.Room != null ? y.Room.Name : null,
                    EquipmentNames = y.Equipment != null ? y.Equipment.Select(z => z.Name).OrderBy(z => z).ToList() : null,
                    RoomDescription = y.Room != null ? y.Room.Description : null
                    
                })
                    .OrderBy(y => y.DateFrom)
                    .ToList()
            })
            .Where(x => x.Specialist.User.Id == request.UserId)
            .OrderBy(x => x.Name)
            .ToList();

        _serviceService.FilterServicesListBy(ref result, request.StringSearch, request.PriceFrom, request.PriceTo, request.ServiceCategoriesIds);

        return result;
    }
}