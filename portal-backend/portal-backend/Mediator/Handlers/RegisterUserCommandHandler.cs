using System.Security.Cryptography;
using MediatR;
using portal_backend.Entities;
using portal_backend.Enums;
using portal_backend.Helpers;
using portal_backend.Mediator.Commands;
using portal_backend.Services;

namespace portal_backend.Mediator.Handlers;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, int>
{
    private readonly VcvsContext _vcvsContext;
    private readonly IConfiguration _config;
    private readonly ImageService _imageService;
    
    public RegisterUserCommandHandler(VcvsContext vcvsContext, IConfiguration config, ImageService imageService)
    {
        
        _vcvsContext = vcvsContext;
        _config = config;
        _imageService = imageService;
    }
    
    public async Task<int> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (!AuthorizationHelpers.ValidateEmail(request.Email)) throw new Exception("Not valid email");
        if (!AuthorizationHelpers.ValidatePassword(request.Password)) throw new Exception("Not valid password");

        var existingUser = _vcvsContext.User.FirstOrDefault(user => user.Email == request.Email || user.UserName == request.UserName);
        if (existingUser != null) throw new Exception("User already exists");

        var dynamicSalt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        switch (request.AccountType)
        {
            case AccountType.Customer:
                request.PhotoFile = null;
                request.PhotoFileExtention = null;
                request.Description = null;
                request.Experience = null;
                request.Education = null;
                break;
            case AccountType.Specialist:
                break;
            case AccountType.Admin:
                request.PhotoFile = null;
                request.PhotoFileExtention = null;
                request.Description = null;
                request.Experience = null;
                request.Education = null;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

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

        switch (request.AccountType)
        {
            case AccountType.Specialist:
                await CreateSpecialist(user, request);
                break;
            case AccountType.Customer:
                CreateCustomer(user);
                break;
            default:
                throw new Exception("Illegal account type");
        }
        

        await _vcvsContext.SaveChangesAsync(cancellationToken);

        return user.Id;
    }

    private async Task CreateSpecialist(User user, RegisterUserCommand request)
    {
        var photoUrl = await _imageService.UploadProfilePhoto(request.PhotoFile, request.PhotoFileExtention);
        
        
        var specialist = new Specialist()
        {
            AgreementId = "S-" + user.UserName,
            Description = request.Description,
            Education = request.Education,
            Experience = request.Experience,
            Photo = photoUrl,
            Services = new List<Service>(),
            User = user
        };

        _vcvsContext.Specialist.Add(specialist);
    }

    private void CreateCustomer(User user)
    {
        var customer = new Customer()
        {
            LastLogin = null,
            LastOrderedService = null,
            LoyaltyGroup = LoyaltyGroup.Bartukas,
            User = user,
            Orders = new List<Order>()
        };

        _vcvsContext.Customer.Add(customer);
    }
}