using MediatR;
using portal_backend.Models;

namespace portal_backend.Mediator.Queries;

public class GetServicesQuery : IRequest<List<ServiceModel>>
{
    public string? StringSearch { get; set; }
    public decimal? PriceFrom { get; set; }
    public decimal? PriceTo { get; set; }
    public List<int>? ServiceCategoriesIds { get; set; }
}