using MediatR;
using portal_backend.Enums;

namespace portal_backend.Mediator.Commands;

public class RegisterUserCommand : IRequest<int>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string City { get; set; }
    public DateTime BirthDate { get; set; }
    public Gender Gender { get; set; }
    public string PhoneNumber { get; set; }
    public AccountType AccountType { get; set; }
    public IFormFile? PhotoFile { get; set; }
    public string? PhotoFileExtention { get; set; }
    public string? Description { get; set; }
    public string? Experience { get; set; }
    public string? Education { get; set; }
}