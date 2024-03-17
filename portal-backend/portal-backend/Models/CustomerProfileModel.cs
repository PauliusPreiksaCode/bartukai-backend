using portal_backend.Enums;

namespace portal_backend.Models;

public class CustomerProfileModel : UserProfileModel
{
    public string? LastOrderedService { get; set; }
    public LoyaltyGroup? LoyaltyGroup { get; set; }
}