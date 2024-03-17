using System.Security.Cryptography;
using MediatR;
using portal_backend.Entities;
using portal_backend.Helpers;
using portal_backend.Mediator.Commands;

namespace portal_backend.Mediator.Handlers;

public class CreateAdminCommandHandler : IRequestHandler<CreateAdminCommand, int>
{
    private readonly VcvsContext _vcvsContext;
    private readonly IConfiguration _config;
    
    public CreateAdminCommandHandler(VcvsContext vcvsContext, IConfiguration config)
    {
        _vcvsContext = vcvsContext;
        _config = config;
    } 
    
    public async Task<int> Handle(CreateAdminCommand request, CancellationToken cancellationToken)
    {
        if (!AuthorizationHelpers.ValidateEmail(request.Email)) throw new Exception("Not valid email");
        if (!AuthorizationHelpers.ValidatePassword(request.Password)) throw new Exception("Not valid password");

        var existingUser = _vcvsContext.User.FirstOrDefault(user => user.Email == request.Email || user.UserName == request.UserName);
        if (existingUser != null) throw new Exception("User already exists");

        var dynamicSalt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.UserName,
            Email = request.Email,
            Password = AuthorizationHelpers.ComputeSha256Hash(request.Password + _config["StaticSalt"] + dynamicSalt),
            Salt = dynamicSalt,
            City = request.City,
            BirthDate = request.BirthDate,
            Gender = request.Gender,
            PhoneNumber = request.PhoneNumber,
            CreationTime = DateTime.Now
        };

        var admin = new Admin()
        {
            User = user,
            WorkerId = request.WorkerId
        };

        _vcvsContext.Admin.Add(admin);
        await _vcvsContext.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}