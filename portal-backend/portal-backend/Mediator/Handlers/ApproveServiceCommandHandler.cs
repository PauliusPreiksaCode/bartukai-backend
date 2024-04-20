using MediatR;
using portal_backend.Mediator.Commands;

namespace portal_backend.Mediator.Handlers;

public class ApproveServiceCommandHandler : IRequestHandler<ApproveServiceCommand>
{
    private readonly VcvsContext _vcvsContext;
    
    public ApproveServiceCommandHandler(VcvsContext vcvsContext)
    {
        _vcvsContext = vcvsContext;
    }
    
    public async Task Handle(ApproveServiceCommand request, CancellationToken cancellationToken)
    {
        var service = _vcvsContext.Service.FirstOrDefault(x => x.Id == request.ServiceId);

        if (service is null)
        {
            throw new Exception("Service doesn't exist");
        }

        service.IsVerified = true;

        await _vcvsContext.SaveChangesAsync(cancellationToken);
    }
}