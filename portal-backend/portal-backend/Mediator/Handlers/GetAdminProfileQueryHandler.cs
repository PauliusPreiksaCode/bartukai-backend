using MediatR;
using portal_backend.Mediator.Queries;
using portal_backend.Models;

namespace portal_backend.Mediator.Handlers;

public class GetAdminProfileQueryHandler : IRequestHandler<GetAdminProfileQuery, AdminProfileModel>
{
    private readonly VcvsContext _vcvsContext;

    public GetAdminProfileQueryHandler(VcvsContext vcvsContext)
    {
        _vcvsContext = vcvsContext;
    }

    public async Task<AdminProfileModel> Handle(GetAdminProfileQuery request, CancellationToken cancellationToken)
    {
        var user = _vcvsContext.User.FirstOrDefault(x => x.Id == request.UserId);

        if (user is null)
        {
            throw new Exception("User doesn't exist");
        }

        var admin = _vcvsContext.Admin.FirstOrDefault(x => x.User.Id == request.UserId);

        if (admin is null)
        {
            throw new Exception("User is not admin");
        }

        return new AdminProfileModel()
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName,
            Email = user.Email,
            City = user.City,
            BirthDate = user.BirthDate,
            Gender = user.Gender,
            PhoneNumber = user.PhoneNumber,
            CreationTime = user.CreationTime,
            WorkerId = admin.WorkerId
        };
    }
}