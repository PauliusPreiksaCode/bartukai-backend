using MediatR;
using portal_backend.Models;

namespace portal_backend.Mediator.Queries;

public class GetSpecialistProfileQuery : IRequest<SpecialistProfileModel>
{
    public int UserId { get; set; }
}