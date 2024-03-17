using MediatR;
using portal_backend.Enums;

namespace portal_backend.Mediator.Queries;

public class GetUserAccountTypeQuery : IRequest<AccountType>
{
    public int UserId { get; set; }
}