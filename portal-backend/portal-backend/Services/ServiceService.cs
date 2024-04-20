using Microsoft.IdentityModel.Tokens;
using portal_backend.Models;

namespace portal_backend.Services;

public class ServiceService
{
    private readonly TimeReservationService _timeReservationService;
    
    public ServiceService(TimeReservationService timeReservationService)
    {
        _timeReservationService = timeReservationService;
    }
    
    public void ValidateInputsForOnline(
        string link,
        List<TimeReservationModel> timeReservations,
        int specialistId,
        List<TimeReservationModel>? toDelete = null)
    {
        if (string.Equals(link, string.Empty))
        {
            throw new MissingFieldException("Empty fields");
        }
        
        _timeReservationService.ValidateLocalReservations(timeReservations);
        timeReservations.ForEach(x => _timeReservationService.ValidateReservation(x, specialistId, toDelete));
    }

    public void ValidateInputsForBusinessCenter(
        List<TimeReservationModel> timeReservations,
        int specialistId,
        List<TimeReservationModel>? toDelete = null)
    {
        _timeReservationService.ValidateLocalReservations(timeReservations);
        timeReservations.ForEach(x => _timeReservationService.ValidateBusinessCenterReservation(x, specialistId, toDelete));
    }

    public void ValidateInputsForOwnedBySpecialist(
        string address, 
        string addressDescription, 
        List<TimeReservationModel> timeReservations,
        int specialistId,
        List<TimeReservationModel>? toDelete = null)
    {
        if (string.Equals(address, string.Empty))
        {
            throw new MissingFieldException("Empty fields");
        }
        
        if (string.Equals(addressDescription, string.Empty))
        {
            throw new MissingFieldException("Empty fields");
        }
        
        _timeReservationService.ValidateLocalReservations(timeReservations);
        timeReservations.ForEach(x => _timeReservationService.ValidateReservation(x, specialistId, toDelete));
    }

    public void FilterServicesListBy(
        ref List<ServiceModel> list,
        string? stringSearch,
        decimal? priceFrom,
        decimal? priceTo,
        List<int>? serviceCategoriesIds)
    {
        if (stringSearch is not null)
        {
            list = list
                .Where(service => service.Name.ToLower().Contains(stringSearch.ToLower()))
                .ToList();
        }

        if (priceFrom is not null)
        {
            list = list
                .Where(service => priceFrom <= service.Price)
                .ToList();
        }
        
        if (priceTo is not null)
        {
            list = list
                .Where(service => service.Price <= priceTo)
                .ToList();
        }

        if (serviceCategoriesIds is not null)
        {
            list = list
                .Where(service =>
                    serviceCategoriesIds.All(id => service.ServiceCategories != null && service.ServiceCategories.Select(y => y.Id).Contains(id)))
                .ToList();
        }
    }
}