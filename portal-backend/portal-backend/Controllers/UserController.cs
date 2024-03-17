using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using portal_backend.Enums;
using portal_backend.Helpers;
using portal_backend.Mediator.Commands;
using portal_backend.Mediator.Queries;

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
}