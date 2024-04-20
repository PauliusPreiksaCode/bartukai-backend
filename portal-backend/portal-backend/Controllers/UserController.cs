using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using portal_backend.Enums;
using portal_backend.Helpers;
using portal_backend.Mediator.Commands;
using portal_backend.Mediator.Queries;
using portal_backend.Models;

namespace portal_backend.Controllers;


public class UserController : BaseController
{
    [HttpGet]
    [Authorize]
    [Route("user")]
    public async Task<IActionResult> GetUserProfile()
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

        try
        {
            switch (accountType)
            {
                case AccountType.Admin:
                    var admin = await Mediator.Send(new GetAdminProfileQuery()
                    {
                        UserId = userId
                    });
                    return Ok(admin);

                case AccountType.Customer:
                    var customer = await Mediator.Send(new GetCustomerProfileQuery()
                    {
                        UserId = userId
                    });
                    return Ok(customer);

                case AccountType.Specialist:
                    var specialist = await Mediator.Send(new GetSpecialistProfileQuery()
                    {
                        UserId = userId
                    });
                    return Ok(specialist);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        catch (Exception e)
        {
            return e.Message switch
            {
                "User doesn't exist" => NotFound("Naudotojas neegzistuoja"),
                _ => StatusCode(500, "Pabandykite vėliau")
            };
        }
    }
    
    [HttpPost]
    [Authorize]
    [Route("service/list")]
    public async Task<IActionResult> GetAllServicesList(GetServicesQuery query)
    {
        try
        {
            var accountType = User.GetAccountType();
            var result = new List<ServiceModel>();
            
            if (!accountType.Equals(AccountType.Specialist))
            {
                result = await Mediator.Send(query);
            }
            else
            {
                result = await Mediator.Send(new GetSpecialistServicesQuery()
                {
                    UserId = User.GetUserId(),
                    StringSearch = query.StringSearch,
                    PriceFrom = query.PriceFrom,
                    PriceTo = query.PriceTo,
                    ServiceCategoriesIds = new List<int>(query.ServiceCategoriesIds ?? new List<int>())
                });
            }
            
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
    [Route("service/categories-list")]
    public async Task<IActionResult> GetAllServiceCategoriesList()
    {
        try
        {
            var result = await Mediator.Send(new GetAllServiceCategoriesQuery());

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
}