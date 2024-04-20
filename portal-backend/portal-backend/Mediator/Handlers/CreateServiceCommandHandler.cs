using System.Transactions;
using MediatR;
using portal_backend.Entities;
using portal_backend.Enums;
using portal_backend.Mediator.Commands;
using portal_backend.Models;
using portal_backend.Services;

namespace portal_backend.Mediator.Handlers;

public class CreateServiceCommandHandler : IRequestHandler<CreateServiceCommand>
{
    private readonly VcvsContext _vcvsContext;
    private readonly TimeReservationService _timeReservationService;
    private readonly ServiceService _serviceService;
    
    public CreateServiceCommandHandler(VcvsContext vcvsContext, TimeReservationService timeReservationService, ServiceService serviceService)
    {
        _vcvsContext = vcvsContext;
        _timeReservationService = timeReservationService;
        _serviceService = serviceService;
    }
    
    public async Task Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        var specialist = _vcvsContext.Specialist.FirstOrDefault(x => x.User.Id == request.UserId);
        if (specialist == null) throw new Exception("Specialist doesn't exist");
        
        var existingService = _vcvsContext.Service.FirstOrDefault(x => 
            string.Equals(x.Name.ToLower(), request.Name.ToLower()) && x.Specialist.Id == specialist.Id);

        if (existingService != null)
        {
            throw new Exception("Service already exists");
        }
        
        var serviceCategories = new List<ServiceCategory>();

        foreach (var serviceCategoryId in request.ServiceCategories.Distinct())
        {
            var serviceCategory = _vcvsContext.ServiceCategory.FirstOrDefault(x => x.Id == serviceCategoryId);

            if (serviceCategory is null)
            {
                throw new Exception("Service category doesn't exist");
            }
            
            serviceCategories.Add(serviceCategory);
        }
        
        switch (request.ServiceLocation)
        {
            case ServiceLocation.Online:
                // Remove extra data
                request.TimeReservations = request.TimeReservations.Select(x => new TimeReservationModel()
                {
                    DateFrom = x.DateFrom,
                    DateTo = x.DateTo,
                    EquipmentIds = new List<int>(),
                    RoomId = null
                }).ToList();
                request.AddressDescription = null;
                request.Address = null;
                
                _serviceService.ValidateInputsForOnline(
                    request.Link ?? "",
                    request.TimeReservations,
                    specialist.Id);
                break;
            
            case ServiceLocation.BusinessCenter:
                // Remove extra data
                request.Link = null;
                request.AddressDescription = null;
                request.Address = null;

                request.TimeReservations = request.TimeReservations.Select(x => new TimeReservationModel()
                    {
                        DateFrom = x.DateFrom,
                        DateTo = x.DateTo,
                        EquipmentIds = (x.EquipmentIds ?? new List<int>()).Distinct().ToList(),
                        RoomId = x.RoomId
                    }
                ).ToList();
                
                _serviceService.ValidateInputsForBusinessCenter(
                    request.TimeReservations,
                    specialist.Id);
                break;
            
            case ServiceLocation.OwnedBySpecialist:
                // Remove extra data
                request.Link = null;
                request.TimeReservations = request.TimeReservations.Select(x => new TimeReservationModel()
                {
                    DateFrom = x.DateFrom,
                    DateTo = x.DateTo,
                    EquipmentIds = new List<int>(),
                    RoomId = null
                }).ToList();
                
                _serviceService.ValidateInputsForOwnedBySpecialist(
                    request.Address ?? "",
                    request.AddressDescription ?? "",
                    request.TimeReservations,
                    specialist.Id);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var service = new Service
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            IsVerified = false,
            Link = request.Link,
            AddressDescription = request.AddressDescription,
            Address = request.Address,
            ServiceLocation = request.ServiceLocation,
            IsAvailable = request.IsAvailable,
            ServiceCategories = serviceCategories,
            Specialist = specialist,
            FullOrders = new List<FullOrder>(),
        };
        
        using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            _vcvsContext.Service.Add(service);
            _timeReservationService.ReserveTimes(request.TimeReservations, service);
            await _vcvsContext.SaveChangesAsync(cancellationToken);

            transactionScope.Complete();
        }

        await _vcvsContext.SaveChangesAsync(cancellationToken);
    }
}