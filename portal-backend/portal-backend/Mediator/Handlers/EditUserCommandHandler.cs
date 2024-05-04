using MediatR;
using Microsoft.EntityFrameworkCore;
using portal_backend.Entities;
using portal_backend.Enums;
using portal_backend.Mediator.Commands;
using portal_backend.Services;

namespace portal_backend.Mediator.Handlers;

public class EditUserCommandHandler : IRequestHandler<EditUserProfileCommand>
{
    private readonly VcvsContext _vcvsContext;
    private readonly IConfiguration _config;
    private readonly ImageService _imageService;
    
    public EditUserCommandHandler(VcvsContext vcvsContext, IConfiguration config, ImageService imageService)
    {
        
        _vcvsContext = vcvsContext;
        _config = config;
        _imageService = imageService;
    }
    
    public async Task Handle(EditUserProfileCommand request, CancellationToken cancellationToken)
    {
        if (request.AccountType == AccountType.Specialist)
        {
            var specialist = _vcvsContext.Specialist
                .Include(s => s.User)
                .FirstOrDefault(s =>
                    s.User.Email == request.Email || s.User.UserName == request.UserName);
            
            UpdateUser(specialist.User, request);

            if (request.PhotoFile is not null)
            {
                var photoUrl = await _imageService.UploadProfilePhoto(request.PhotoFile, request.PhotoFileExtention);
                specialist.Photo = photoUrl;
            }

            specialist.Description = request.Description;
            specialist.Education = request.Education;
            specialist.Experience = request.Experience;
        }
        else
        {
            var user = _vcvsContext.User
                .FirstOrDefault(user => user.Email == request.Email || user.UserName == request.UserName);
            
            UpdateUser(user, request);
        }
        
        await _vcvsContext.SaveChangesAsync(cancellationToken);
    }

    void UpdateUser(User user, EditUserProfileCommand request)
    {
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.City = request.City;
        user.BirthDate = request.BirthDate;
        user.Gender = request.Gender;
        user.PhoneNumber = request.PhoneNumber;
    }
}