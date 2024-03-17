using MediatR;
using portal_backend.Mediator.Queries;
using portal_backend.Models;

namespace portal_backend.Mediator.Handlers;

public class GetSpecialistProfileQueryHandler : IRequestHandler<GetSpecialistProfileQuery, SpecialistProfileModel>
{
    private readonly VcvsContext _vcvsContext;
    
    public GetSpecialistProfileQueryHandler(VcvsContext vcvsContext)
    {
        _vcvsContext = vcvsContext;
    }
    
    public async Task<SpecialistProfileModel> Handle(GetSpecialistProfileQuery request, CancellationToken cancellationToken)
    {
        var user = _vcvsContext.User.FirstOrDefault(x => x.Id == request.UserId);

        if (user is null)
        {
            throw new Exception("User doesn't exist");
        }

        var specialist = _vcvsContext.Specialist.FirstOrDefault(x => x.User.Id == request.UserId);

        if (specialist is null)
        {
            throw new Exception("User is not specialist");
        }

        return new SpecialistProfileModel()
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
            Photo = specialist.Photo,
            Description = specialist.Description,
            Experience = specialist.Experience,
            Education = specialist.Education,
            AgreementId = specialist.AgreementId
        };
    }
}