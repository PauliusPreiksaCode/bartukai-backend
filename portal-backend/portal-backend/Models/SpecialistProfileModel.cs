namespace portal_backend.Models;

public class SpecialistProfileModel : UserProfileModel
{
    public string? Photo { get; set; }
    public string? Description { get; set; }
    public string? Experience { get; set; }
    public string? Education { get; set; }
    public string? AgreementId { get; set; }
}