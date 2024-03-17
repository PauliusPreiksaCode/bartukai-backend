using System.Security.Claims;
using portal_backend.Enums;
using static System.Enum;

namespace portal_backend.Helpers;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        return int.Parse(user?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value!);;
    }
    
    public static AccountType GetAccountType(this ClaimsPrincipal user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        var isParsable = TryParse(user?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value!, out AccountType accountType);

        if (!isParsable)
        {
            throw new ArgumentNullException(nameof(user));
        }
        
        return accountType;
    }
}