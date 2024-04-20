using MediatR;
using portal_backend.Mediator.Queries;
using portal_backend.Models;

namespace portal_backend.Mediator.Handlers;

public class GetAllServiceCategoriesQueryHandler : IRequestHandler<GetAllServiceCategoriesQuery, List<ServiceCategoryModel>>
{
    private readonly VcvsContext _vcvsContext;

    public GetAllServiceCategoriesQueryHandler(VcvsContext vcvsContext)
    {
        _vcvsContext = vcvsContext;
    }
    
    public async Task<List<ServiceCategoryModel>> Handle(GetAllServiceCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categoriesDb = _vcvsContext.ServiceCategory.ToList();

        var result = categoriesDb.Select(x => new ServiceCategoryModel()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            
            })
            .OrderBy(x => x.Name)
            .ToList();

        return result;
    }
}