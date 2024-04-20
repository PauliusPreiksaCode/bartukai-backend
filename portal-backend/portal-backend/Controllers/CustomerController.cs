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
    private readonly IConfiguration _config;

    public CustomerController(IConfiguration config)
    {
        _config = config;
    }
    
    [AllowAnonymous]
    [HttpPost]
    [Route("super-duper-secret/create-admin")]
    public async Task<IActionResult> CreateAdmin([FromBody]CreateAdminCommand command)
    {
        try
        {
            await Mediator.Send(command);
            return Ok();
        }
        catch(Exception exception)
        {
            return exception.Message switch
            {
                "Not valid email" => BadRequest("Neteisingas el. pašto adresas"),
                "Not valid password" => BadRequest("Blogas slaptažodis"),
                "User already exists" => Conflict("Toks naudotojas jau egzistuoja"),
                _ => StatusCode(500, "Pabandykite vėliau")
            };  
        }
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> RegisterUser([FromForm] RegisterUserCommand command)
    {
        try
        {
            var userId = await Mediator.Send(command);
            await Mediator.Send(new SendRegisterEmailCommand()
            {
                UserId = userId
            });
            return Ok();
        }
        catch (Exception e)
        {
            return e.Message switch
            {
                "Not valid email" => BadRequest("Neteisingas el. pašto adresas"),
                "Not valid password" => BadRequest("Blogas slaptažodis"),
                "User already exists" => Conflict("Toks naudotojas jau egzistuoja"),
                "Illegal account type" => BadRequest("Neteisingas paskyros tipas"),
                "Register email cannot be sent to user that doesn't exist" => Ok(),
                "Something with email" => Ok(),
                _ => StatusCode(500, "Pabandykite vėliau")
            };  
        }
    }
    
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

    [AllowAnonymous]
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> LoginUser(
        [FromBody] LoginUserCommand command)
    {
        try
        {
            var userId = await Mediator.Send(command);

            if (userId is not null)
            {
                await Mediator.Send(new SetLastLoginDateCommand()
                {
                    UserId = userId.Value,
                    LastLoginDate = DateTime.Now
                });
                return Ok(await GenerateTokens(userId.Value));
            }

            return Unauthorized();
        }
        catch(Exception exception)
        {
            return exception.Message switch
            {
                "User not found" => Unauthorized("Neteisingas naudotojo vardas arba slaptažodis"),
                _ => StatusCode(500, "Pabandykite vėliau")
            };
        }
    }

    [HttpGet]
    [Authorize]
    [Route("renew-token")]
    public async Task<IActionResult> RenewToken()
    {
        int userId;
        try
        {
            userId = User.GetUserId();
        }
        catch (Exception e)
        {
            return BadRequest("Nepavyko atnaujinti prisijungimo galiojimo");
        }

        return Ok(await GenerateTokens(userId));
    }

    private async Task<TokenModel> GenerateTokens(int userId)
    {
        return new TokenModel()
        {
            Token = await GenerateToken(userId),
            RefreshToken = await GenerateToken(userId, true)
        };
    }

    private async Task<string> GenerateToken(int userId, bool isRefresh = false)
    {
        var accountType = await Mediator.Send(new GetUserAccountTypeQuery()
        {
            UserId = userId
        });

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpireMinutes"]!));

        if (isRefresh)
        {
            expires = expires.AddMinutes(int.Parse(_config["Jwt:ExpireMinutes"]!));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, userId.ToString()),
                new Claim(ClaimTypes.Role, ((int)accountType).ToString() ?? throw new InvalidOperationException())
            }),
            IssuedAt = DateTime.UtcNow,
            Expires = expires,
            SigningCredentials = credentials,
            Issuer = _config["Jwt:Issuer"]!,
            Audience = _config["Jwt:Audience"]!
        };

        var token = new JwtSecurityTokenHandler().CreateToken(tokenDescriptor);

        return new JwtSecurityTokenHandler().WriteToken(token);

    }
}