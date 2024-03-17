using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using portal_backend.Enums;
using portal_backend.Helpers;
using portal_backend.Mediator.Commands;
using portal_backend.Mediator.Queries;

namespace portal_backend.Controllers;

public class AdministratorController : BaseController
{
    [HttpGet]
    [Authorize]
    [Route("room/list")]
    public async Task<IActionResult> GetAllRoomsList()
    {

        try
        {
            var accountType = User.GetAccountType();

            if (!accountType.Equals(AccountType.Admin))
            {
                return new ForbidResult();
            }
        
            var result = await Mediator.Send(new GetAllRoomsQuery());

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
    [Route("room")]
    public async Task<IActionResult> CreateRoom(CreateRoomCommand command)
    {
        try
        {
            var accountType = User.GetAccountType();

            if (!accountType.Equals(AccountType.Admin))
            {
                return new ForbidResult();
            }
            
            await Mediator.Send(command);
            return Ok();
        }
        catch (Exception exception)
        {
            return exception.Message switch
            {
                "Room already exists" => Conflict("Toks kambarys jau egzistuoja"),
                _ => StatusCode(500, "Pabandykite vėliau")
            };
        }
    }
    
    [HttpPut]
    [Authorize]
    [Route("room")]
    public async Task<IActionResult> EditRoom(EditRoomCommand command)
    {
        try
        {
            var accountType = User.GetAccountType();

            if (!accountType.Equals(AccountType.Admin))
            {
                return new ForbidResult();
            }
            
            await Mediator.Send(command);
            return Ok();
        }
        catch (Exception exception)
        {
            return exception.Message switch
            {
                "Room doesn't exist" => NotFound("Toks kambarys neegzistuoja"),
                "Room name is already used by other room" => Conflict("Toks kambario pavadinimas jau užimtas"),
                _ => StatusCode(500, "Pabandykite vėliau")
            };
        }
    }
    
    [HttpDelete]
    [Authorize]
    [Route("room/{id}")]
    public async Task<IActionResult> DeleteRoom(int id)
    {
        try
        {
            var accountType = User.GetAccountType();

            if (!accountType.Equals(AccountType.Admin))
            {
                return new ForbidResult();
            }
        
            var status = await Mediator.Send(new DeleteRoomCommand()
            {
                RoomId = id
            });

            if (status)
            {
                return Ok();
            }

            return Ok("Tik laisvi laikai buvo ištrinti");
        }
        catch (Exception e)
        {
            return e.Message switch
            {
                "Room doesn't exist" => NotFound("Toks kambarys neegzistuoja"),
                _ => StatusCode(500, "Pabandykite vėliau")
            };
        }
    }
    
    [HttpGet]
    [Authorize]
    [Route("equipment/list")]
    public async Task<IActionResult> GetAllEquipmentList()
    {
        try
        {
            var accountType = User.GetAccountType();

            if (!accountType.Equals(AccountType.Admin))
            {
                return new ForbidResult();
            }

            var result = await Mediator.Send(new GetAllEquipmentQuery());

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
    [Route("equipment")]
    public async Task<IActionResult> CreateEquipment(CreateEquipmentCommand command)
    {
        var accountType = User.GetAccountType();

        if (!accountType.Equals(AccountType.Admin))
        {
            return new ForbidResult();
        }

        try
        {
            await Mediator.Send(command);
            return Ok();
        }
        catch (Exception exception)
        {
            return exception.Message switch
            {
                "Equipment already exists" => Conflict("Tokia įranga jau naudojama"),
                _ => StatusCode(500, "Pabandykite vėliau")
            };
        }
    }
    
    [HttpPut]
    [Authorize]
    [Route("equipment")]
    public async Task<IActionResult> EditEquipment(EditEquipmentCommand command)
    {
        var accountType = User.GetAccountType();

        if (!accountType.Equals(AccountType.Admin))
        {
            return new ForbidResult();
        }

        try
        {
            await Mediator.Send(command);
            return Ok();
        }
        catch (Exception exception)
        {
            return exception.Message switch
            {
                "Equipment doesn't exist" => NotFound("Įranga neegzistuoja"),
                "Equipment inventory number is already used by other equipment" => Conflict("Įrangos inventorinis numeris jau yra panaudotas"),
                _ => StatusCode(500, "Pabandykite vėliau")
            };
        }
    }

    [HttpDelete]
    [Authorize]
    [Route("equipment/{id}")]
    public async Task<IActionResult> DeleteEquipment(int id)
    {
        try
        {
            var accountType = User.GetAccountType();

            if (!accountType.Equals(AccountType.Admin))
            {
                return new ForbidResult();
            }
        
            var status = await Mediator.Send(new DeleteEquipmentCommand()
            {
                EquipmentId = id
            });

            if (status)
            {
                return Ok();
            }

            return Ok("Buvo ištrinti tik laisvi laikai");
        }
        catch (Exception e)
        {
            return e.Message switch
            {
                "Equipment doesn't exist" => NotFound("Įranga nerasta"),
                _ => StatusCode(500, "Pabandykite vėliau")
            }; 
        }
    }
}