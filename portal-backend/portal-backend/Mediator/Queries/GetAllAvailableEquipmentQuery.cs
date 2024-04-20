using MediatR;
using portal_backend.Entities;
using portal_backend.Models;

namespace portal_backend.Mediator.Queries;

public class GetAllAvailableEquipmentQuery : IRequest<List<EquipmentModel>>
{
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
}