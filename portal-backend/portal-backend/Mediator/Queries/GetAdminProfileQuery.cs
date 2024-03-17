using MediatR;
using portal_backend.Models;

namespace portal_backend.Mediator.Queries;

public class GetAdminProfileQuery : IRequest<AdminProfileModel>
{
    public int UserId { get; set; }
}