using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using portal_backend.Enums;
using portal_backend.Helpers;
using portal_backend.Mediator.Commands;
using portal_backend.Mediator.Queries;
using portal_backend.Models;

namespace portal_backend.Controllers;

public class SpecialistController : BaseController
{
    [HttpGet]
    [Authorize]
    [Route("room/available-list")]
    public async Task<IActionResult> GetAllAvailableRoomsList(DateTime dateFrom, DateTime dateTo)
    {
        try
        {
            var accountType = User.GetAccountType();

            if (!accountType.Equals(AccountType.Specialist))
            {
                return new ForbidResult();
            }
        
            var result = await Mediator.Send(new GetAllAvailableRoomsQuery()
            {
                DateFrom = dateFrom,
                DateTo = dateTo
            });

            return Ok(result);
        }
        catch (Exception e)
        {
            return e.Message switch
            {
                _ => StatusCode(500, "Pabandykite vėliau")
            };
        }
    }
    
    [HttpGet]
    [Authorize]
    [Route("equipment/available-list")]
    public async Task<IActionResult> GetAllAvailableEquipmentList(DateTime dateFrom, DateTime dateTo)
    {
        try
        {
            var accountType = User.GetAccountType();

            if (!accountType.Equals(AccountType.Specialist))
            {
                return new ForbidResult();
            }
        
            var result = await Mediator.Send(new GetAllAvailableEquipmentQuery()
            {
                DateFrom = dateFrom,
                DateTo = dateTo
            });

            return Ok(result);
        }
        catch (Exception e)
        {
            return e.Message switch
            {
                _ => StatusCode(500, "Pabandykite vėliau")
            }; 
        }
    }

    [HttpPost]
    [Authorize]
    [Route("service")]
    public async Task<IActionResult> CreateService(CreateServiceRequest request)
    {
        int userId;
        AccountType accountType;
        try
        {
            userId = User.GetUserId();
            accountType = User.GetAccountType();
        }
        catch (Exception e)
        {
            return BadRequest("Pabandykite vėliau");
        }

        if (!accountType.Equals(AccountType.Specialist))
        {
            return new ForbidResult();
        }

        var command = new CreateServiceCommand()
        {
            UserId = userId,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Link = request.Link,
            AddressDescription = request.AddressDescription,
            Address = request.Address,
            ServiceLocation = request.ServiceLocation,
            IsAvailable = request.IsAvailable,
            ServiceCategories = new List<int>(request.ServiceCategories),
            TimeReservations = new List<TimeReservationModel>(request.TimeReservations),
        };

        try
        {
            await Mediator.Send(command);
            return Ok();
        }
        catch (Exception exception)
        {
            return exception.Message switch
            {
                "Specialist doesn't exist" => NotFound("Specialistas neegzistuoja"),
                "Service already exists" => Conflict("Paslauga jau egzistuoja"),
                "Service category doesn't exist" => NotFound("Paslaugos kategorija neegzistuoja"),
                "Empty fields" => BadRequest("Ne visi laukai buvo užpildyti"),
                "Provided reservations happens at the same time" => Conflict("Pateiktos rezervacijos vyksta tuo pačiu metu"),
                "TimeReservation model has DateFrom >= DateTo" => BadRequest("Data nuo yra vėliau nei data iki"),
                "Reservation cannot happen in the past" => BadRequest("Rezervacija negali vykti praeityje"),
                "Time is already in use" => Conflict("Rezervacijos laikas jau užimtas"),
                "Room doesn't exist" => NotFound("Toks kambarys neegzistuoja"),
                "Room is not available" => BadRequest("Kambarys nepasiekiamas"),
                "Room is already in use" => Conflict("Kambarys jau užimtas"),
                "Equipment doesn't exist" => NotFound("Įranga neegzistuoja"),
                "Equipment is not available" => BadRequest("Įranga nepasiekiama"),
                "Equipment is already in use" => Conflict("Įranga jau užimta"),
                "Reservation doesn't exist" => NotFound("Rezervacija neegzistuoja"),
                "Reservation happened in the past" => BadRequest("Rezervacija jau yra įvykusi"),
                _ => StatusCode(500)
            };
        }
    }
    
    [HttpPut]
    [Authorize]
    [Route("service")]
    public async Task<IActionResult> UpdateService(UpdateServiceRequest request)
    {
        try
        {
            var accountType = User.GetAccountType();

            if (!accountType.Equals(AccountType.Specialist))
            {
                return new ForbidResult();
            }
        
            await Mediator.Send(new UpdateServiceCommand()
            {
                UserId = User.GetUserId(),
                Id = request.Id,
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Link = request.Link,
                AddressDescription = request.AddressDescription,
                Address = request.Address,
                IsAvailable = request.IsAvailable,
                ServiceCategories = new List<int>(request.ServiceCategories),
                TimeReservations = new List<TimeReservationModel>(request.TimeReservations) 
            });

            return Ok();
        }
        catch (Exception e)
        {
            return e.Message switch
            {
                "Specialist doesn't exist" => NotFound("Specialistas neegzistuoja"),
                "Service with this name already exists" => Conflict("Paslauga su tokiu pavadinimu jau egzistuoja"),
                "Service already exists" => Conflict("Paslauga jau egzistuoja"),
                "Service doesn't exist" => NotFound("Paslauga neegizstuoja"),
                "Service category doesn't exist" => NotFound("Paslaugos kategorija neegzistuoja"),
                "Empty fields" => BadRequest("Ne visi laukai buvo užpildyti"),
                "Provided reservations happens at the same time" => Conflict("Pateiktos rezervacijos vyksta tuo pačiu metu"),
                "TimeReservation model has DateFrom >= DateTo" => BadRequest("Data nuo yra vėliau nei data iki"),
                "Reservation cannot happen in the past" => BadRequest("Rezervacija negali vykti praeityje"),
                "Time is already in use" => Conflict("Rezervacijos laikas jau užimtas"),
                "Room doesn't exist" => NotFound("Toks kambarys neegzistuoja"),
                "Room is not available" => BadRequest("Kambarys nepasiekiamas"),
                "Room is already in use" => Conflict("Kambarys jau užimtas"),
                "Equipment doesn't exist" => NotFound("Įranga neegzistuoja"),
                "Equipment is not available" => BadRequest("Įranga nepasiekiama"),
                "Equipment is already in use" => Conflict("Įranga jau užimta"),
                "Reservation doesn't exist" => NotFound("Rezervacija neegzistuoja"),
                "Reservation happened in the past" => BadRequest("Rezervacija jau yra įvykusi"),
                "Reservations are not part of this service" => Conflict("Rezervacija nepriklauso šiai paslaugai"),
                "More than one reservation with same id found" => BadRequest("Daugiau nei viena tokia pati rezervacija rasta"),
                "Reservation is too old" => BadRequest("Rezervacija per sena"),
                _ => StatusCode(500)
            };
        }
    }
    
    [HttpDelete]
    [Authorize]
    [Route("service/{id}")]
    public async Task<IActionResult> DeleteService(int id)
    {
        try
        {
            var accountType = User.GetAccountType();

            if (!accountType.Equals(AccountType.Specialist))
            {
                return new ForbidResult();
            }
        
            var status = await Mediator.Send(new DeleteServiceCommand()
            {
                UserId = User.GetUserId(),
                ServiceId = id,
            });

            if (status)
            {
                return Ok();
            }

            return Ok("Pašalinti tik laisvi laikai");
        }
        catch (Exception e)
        {
            return e.Message switch
            {
                "Service doesn't exist" => NotFound("Paslauga neegzistuoja"),
                _ => StatusCode(500, "Pabandykite vėliau")
            };
        }
    }
}