using MediatR;
using portal_backend.Models;

namespace portal_backend.Mediator.Queries;

public class GetSpecialistServicesQuery : IRequest<List<ServiceModel>>
{
    public int UserId { get; set; }
    public string? StringSearch { get; set; }
    public decimal? PriceFrom { get; set; }
    public decimal? PriceTo { get; set; }
    public List<int>? ServiceCategoriesIds { get; set; }
}