using MediatR;
using Microsoft.EntityFrameworkCore;
using portal_backend.Entities;
using portal_backend.Mediator.Queries;
using portal_backend.Models;
using portal_backend.Services;

namespace portal_backend.Mediator.Handlers;

public class GetNonApprovedServicesQueryHandler : IRequestHandler<GetNonApprovedServicesListQuery, List<ServiceModel>>
{
    private readonly VcvsContext _vcvsContext;
    
    public GetNonApprovedServicesQueryHandler(VcvsContext vcvsContext)
    {
        _vcvsContext = vcvsContext;
    }

    public async Task<List<ServiceModel>> Handle(GetNonApprovedServicesListQuery request, CancellationToken cancellationToken)
    {
        var data = _vcvsContext.Service
            .Include(service => service.Specialist)
            .Include(service => service.Specialist.User)
            .Include(service => service.ServiceCategories)
            .Include(service => service.FullOrders);
        
        var result = data
            .Where(x => !x.IsVerified)
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
                TimeReservations = x.FullOrders
                    .Where(y => y.OrderId == null)
                    .Select(y => new TimeReservationModel()
                {
                    Id = y.Id,
                    DateFrom = y.DateFrom,
                    DateTo = y.DateTo,
                    RoomId = y.Room != null ? y.Room.Id : null,
                    EquipmentIds = y.Equipment != null ? y.Equipment.Select(z => z.Id).ToList() : null,
                    RoomName = y.Room != null ? y.Room.Name : ""
                })
                    .OrderBy(y => y.DateFrom)
                    .ToList()
            })
            .OrderBy(x => x.Name)
            .ToList();

        return result;
    }
}