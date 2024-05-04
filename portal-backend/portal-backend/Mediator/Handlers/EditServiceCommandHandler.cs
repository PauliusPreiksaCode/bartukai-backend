using System.Transactions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using portal_backend.Entities;
using portal_backend.Enums;
using portal_backend.Mediator.Commands;
using portal_backend.Models;
using portal_backend.Services;

namespace portal_backend.Mediator.Handlers;

public class EditServiceCommandHandler : IRequestHandler<EditServiceCommand>
{
    private readonly VcvsContext _vcvsContext;
    private readonly ServiceService _serviceService;
    private readonly TimeReservationService _timeReservationService;

    public EditServiceCommandHandler(VcvsContext vcvsContext, ServiceService serviceService, TimeReservationService timeReservationService)
    {
        _vcvsContext = vcvsContext;
        _serviceService = serviceService;
        _timeReservationService = timeReservationService;
    }

    public async Task Handle(EditServiceCommand request, CancellationToken cancellationToken)
    {
        var specialist = _vcvsContext.Specialist.FirstOrDefault(x => x.User.Id == request.UserId);
        if (specialist == null) throw new Exception("Specialist doesn't exist");
        
        var existingService = _vcvsContext.Service.FirstOrDefault(x => 
            string.Equals(x.Name.ToLower(), request.Name.ToLower()) && x.Specialist.Id == specialist.Id && x.Id != request.Id);

        if (existingService != null)
        {
            throw new Exception("Service with this name already exists");
        }
        
        var service = _vcvsContext.Service
            .Include(x => x.FullOrders)
            .Include(x => x.ServiceCategories)
            .FirstOrDefault(x => x.Specialist.Id == specialist.Id && x.Id == request.Id);

        if (service is null)
        {
            throw new Exception("Service doesn't exist");
        }
        
        var givenServiceCategories = new List<ServiceCategory>();

        foreach (var serviceCategoryId in request.ServiceCategories.Distinct())
        {
            var serviceCategory = _vcvsContext.ServiceCategory.FirstOrDefault(x => x.Id == serviceCategoryId);

            if (serviceCategory is null)
            {
                throw new Exception("Service category doesn't exist");
            }
            
            givenServiceCategories.Add(serviceCategory);
        }

        var categoriesToDelete = service.ServiceCategories
            .Where(x => !givenServiceCategories
                .Select(y => y.Id)
                .Contains(x.Id))
            .ToList();
        
        var categoriesToAdd = givenServiceCategories
            .Where(x => !service.ServiceCategories
                .Select(y => y.Id)
                .Contains(x.Id))
            .ToList();

        var originalExistingTime = _vcvsContext.FullOrder
            .Where(x => x.Service.Specialist.User.Id == request.UserId && x.Service.Id == request.Id && x.DateFrom > DateTime.Now)
            .ToList();

        var givenExistingTimeReservations = request.TimeReservations
            .Where(x => x.Id is not null)
            .ToList();

        if (!givenExistingTimeReservations
                .Select(x => x.Id ?? -1)
                .All(x => originalExistingTime
                    .Select(y => y.Id)
                    .Contains(x)))
        {
            throw new Exception("Reservations are not part of this service");
        }

        if (givenExistingTimeReservations.Count != givenExistingTimeReservations.Distinct().Count())
        {
            throw new Exception("More than one reservation with same id found");
        }
        
        if (givenExistingTimeReservations.Count != givenExistingTimeReservations.Count(x => x.DateFrom > DateTime.Now))
        {
            throw new Exception("Reservation is too old");
        }

        var reservationsToDelete = originalExistingTime
            .Where(x => !givenExistingTimeReservations
                .Select(y => y.Id)
                .Contains(x.Id))
            .Select(x => new TimeReservationModel()
            {
                Id = x.Id,
                DateFrom = x.DateFrom,
                DateTo = x.DateTo,
                EquipmentIds = x.Equipment != null ? x.Equipment.Select(y => y.Id).ToList() : new List<int>(),
                RoomId = x.Room?.Id
            })
            .ToList();
        
        var reservationsToEdit = givenExistingTimeReservations
            .Where(x => originalExistingTime
                .Select(y => y.Id)
                .Contains(x.Id ?? -1))
            .ToList();

        var reservationsToAdd = request.TimeReservations
            .Where(x => x.Id is null)
            .ToList();

        RemoveServiceExtraDataBasedOnServiceLocation(ref request, service.ServiceLocation);
        
        RemoveTimeReservationsExtraDataBasedOnServiceLocation(ref reservationsToEdit, service.ServiceLocation);
        RemoveTimeReservationsExtraDataBasedOnServiceLocation(ref reservationsToAdd, service.ServiceLocation);

        var exclude = new List<TimeReservationModel>(reservationsToEdit);
        exclude.AddRange(reservationsToDelete);
        ValidateInputBasedOnServiceLocation(request, service.ServiceLocation, specialist, exclude);

        using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            async void RemoveReservation(TimeReservationModel x) => await _timeReservationService.RemoveReservation(x.Id ?? -1, request.UserId);
            async void UpdateReservation(TimeReservationModel x) => await _timeReservationService.UpdateReservation(x, request.UserId);
            
            service.Name = request.Name;
            service.Description = request.Description;
            service.Price = request.Price;
            service.Link = request.Link;
            service.AddressDescription = request.AddressDescription;
            service.Address = request.Address;
            service.IsAvailable = request.IsAvailable;
            service.Specialist = specialist;

            foreach (var serviceCategory in categoriesToDelete)
            {
                service.ServiceCategories.Remove(serviceCategory);
            }

            foreach (var serviceCategory in categoriesToAdd)
            {
                service.ServiceCategories.Add(serviceCategory);
            }
            
            reservationsToDelete.ForEach(RemoveReservation);
            reservationsToEdit.ForEach(UpdateReservation);
            _timeReservationService.ReserveTimes(reservationsToAdd, service);
            
            await _vcvsContext.SaveChangesAsync(cancellationToken);

            transactionScope.Complete();
        }

        await _vcvsContext.SaveChangesAsync(cancellationToken);
    }

    private void RemoveServiceExtraDataBasedOnServiceLocation(ref EditServiceCommand request, ServiceLocation serviceLocation)
    {
        switch (serviceLocation)
        {
            case ServiceLocation.Online:
                request.AddressDescription = null;
                request.Address = null;
                break;

            case ServiceLocation.BusinessCenter:
                request.Link = null;
                request.AddressDescription = null;
                request.Address = null;
                break;

            case ServiceLocation.OwnedBySpecialist:
                request.Link = null;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private void RemoveTimeReservationsExtraDataBasedOnServiceLocation(ref List<TimeReservationModel> timeReservations, ServiceLocation serviceLocation)
    {
        switch (serviceLocation)
        {
            case ServiceLocation.Online:
                timeReservations = timeReservations.Select(x => new TimeReservationModel()
                {
                    Id = x.Id,
                    DateFrom = x.DateFrom,
                    DateTo = x.DateTo,
                    EquipmentIds = new List<int>(),
                    RoomId = null
                }).ToList();
                break;

            case ServiceLocation.BusinessCenter:
                timeReservations = timeReservations.Select(x => new TimeReservationModel()
                    {
                        Id = x.Id,
                        DateFrom = x.DateFrom,
                        DateTo = x.DateTo,
                        EquipmentIds = (x.EquipmentIds ?? new List<int>()).Distinct().ToList(),
                        RoomId = x.RoomId
                    }
                ).ToList();
                break;

            case ServiceLocation.OwnedBySpecialist:
                timeReservations = timeReservations.Select(x => new TimeReservationModel()
                {
                    Id = x.Id,
                    DateFrom = x.DateFrom,
                    DateTo = x.DateTo,
                    EquipmentIds = new List<int>(),
                    RoomId = null
                }).ToList();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private void ValidateInputBasedOnServiceLocation(
        EditServiceCommand request,
        ServiceLocation serviceLocation,
        Specialist specialist,
        List<TimeReservationModel> exclude)
    {
        switch (serviceLocation)
        {
            case ServiceLocation.Online:
                _serviceService.ValidateInputsForOnline(
                    request.Link ?? "",
                    request.TimeReservations,
                    specialist.Id,
                    exclude);
                break;

            case ServiceLocation.BusinessCenter:
                _serviceService.ValidateInputsForBusinessCenter(
                    request.TimeReservations,
                    specialist.Id,
                    exclude);
                break;

            case ServiceLocation.OwnedBySpecialist:
                _serviceService.ValidateInputsForOwnedBySpecialist(
                    request.Address ?? "",
                    request.AddressDescription ?? "",
                    request.TimeReservations,
                    specialist.Id,
                    exclude);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}