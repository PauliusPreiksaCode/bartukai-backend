using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using portal_backend.Enums;
using portal_backend.Helpers;
using portal_backend.Mediator.Commands;
using portal_backend.Mediator.Queries;
using portal_backend.Models;

namespace portal_backend.Controllers;

public class CustomerController : BaseController
{
    [HttpGet]
    [Authorize]
    [Route("order/my-list")]
    public async Task<IActionResult> GetAllOrdersListForCustomer()
    {
        try
        {
            var accountType = User.GetAccountType();

            if (!accountType.Equals(AccountType.Customer))
            {
                return new ForbidResult();
            }


            var result = await Mediator.Send(new GetAllCustomerOrdersListQuery()
            {
                UserId = User.GetUserId()
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
    [Route("service/unused-times/{id}")]
    [Authorize]
    public async Task<IActionResult> GetUnusedServiceTimes(int id)
    {
        try
        {
            var accountType = User.GetAccountType();

            if (!accountType.Equals(AccountType.Customer))
            {
                return new ForbidResult();
            }
        
            var result = await Mediator.Send(new GetUnusedServiceTimesQuery()
            {
                ServiceId = id
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
    [Route("order")]
    public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
    {
        try
        {
            var accountType = User.GetAccountType();

            if (!accountType.Equals(AccountType.Customer))
            {
                return new ForbidResult();
            }

            var orderId = await Mediator.Send(new CreateOrderCommand()
            {
                UserId = User.GetUserId(),
                TimeReservationId = request.TimeReservationId,
                PaymentType = request.PaymentType
            });
        
            await Mediator.Send(new SendOrderEmailCommand()
            {
                UserId = User.GetUserId(),
                OrderId = orderId
            });
        
            return Ok();
        }
        catch (Exception e)
        {
            return e.Message switch
            {
                "Time reservation doesn't exist" => NotFound("Rezervacijos laikas neegzistuoja"),
                "Time already reserved" => Conflict("Laikas jau užimtas"),
                "Time in the past" => BadRequest("Laikas praeityje"),
                "Service is unavailable" => BadRequest("Paslauga nepasiekiama"),
                "Customer doesn't exist" => NotFound("Klientas neegzistuoja"),
                "Email failed" => Ok(),
                _ => StatusCode(500, "Pabandykite vėliau")
            }; 
        }
    }
    
    [HttpDelete]
    [Authorize]
    [Route("order/{id}")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        try
        {
            var accountType = User.GetAccountType();

            if (!accountType.Equals(AccountType.Customer))
            {
                return new ForbidResult();
            }


            await Mediator.Send(new CancelOrderCommand()
            {
                UserId = User.GetUserId(),
                OrderId = id
            });

            return Ok();
        }
        catch (Exception e)
        {
            return e.Message switch
            {
                "Order doesn't exist" => NotFound("Užsakymas neegzistuoja"),
                "It's not user's order" => Conflict("Naudotojui nepriklauso užsakymas"),
                "Time in the past" => BadRequest("Laikas praeityje"),
                "Order already canceled" => Conflict("Užsakymas jau atšauktas"),
                "Reservation doesn't exist" => NotFound("Rezervacija neegzistuoja"),
                _ => StatusCode(500, "Pabandykite vėliau")
            };
        }
    }
    
    [HttpPut]
    [Authorize]
    [Route("order")]
    public async Task<IActionResult> EditOrder(EditOrderRequest request)
    {
        try
        {
            var accountType = User.GetAccountType();

            if (!accountType.Equals(AccountType.Customer))
            {
                return new ForbidResult();
            }


            await Mediator.Send(new EditOrderCommand()
            {
                UserId = User.GetUserId(),
                OrderId = request.OrderId,
                NewReservationId = request.NewReservationId,
                PaymentType = request.PaymentType
            });

            return Ok();
        }
        catch (Exception e)
        {
            return e.Message switch
            {
                "Order doesn't exist" => NotFound("Užsakymas neegzistuoja"),
                "It's not user's order" => Conflict("Naudotojui nepriklauso užsakymas"),
                "Order is canceled" => Conflict("Užsakymas jau atšauktas"),
                "Order happened in the past" => BadRequest("Užsakymas įvyko praeityje"),
                "Reservation time doesn't exist" => NotFound("Rezervacijos laikas neegzistuoja"),
                "Reservation happened in the past" => BadRequest("Rezervacija įvyko praeityje"),
                "Can't change to other service" => Conflict("Negalima pakeisti laiko į kitos paslaugos"),
                "Reservation already occupied" => Conflict("Rezervacijos laikas jau yra naudojamas"),
                _ => StatusCode(500, "Pabandykite vėliau")
            };
        }
    }
}