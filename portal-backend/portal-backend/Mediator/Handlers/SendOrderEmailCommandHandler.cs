using MediatR;
using Microsoft.EntityFrameworkCore;
using portal_backend.Entities;
using portal_backend.Enums;
using portal_backend.Mediator.Commands;
using portal_backend.Models;
using portal_backend.Services;

namespace portal_backend.Mediator.Handlers;

public class SendOrderEmailCommandHandler : IRequestHandler<SendOrderEmailCommand>
{
    private readonly VcvsContext _vcvsContext;
    private readonly EmailService _emailService;

    public SendOrderEmailCommandHandler(VcvsContext vcvsContext, EmailService emailService)
    {
        _vcvsContext = vcvsContext;
        _emailService = emailService;
    }

    public async Task Handle(SendOrderEmailCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var mailBodyTemplate= @"
<!DOCTYPE html>
<html lang='lt'>

<body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>

<table style='max-width: 600px; margin: 0 auto; background-color: #fff; padding: 20px; border-radius: 8px;'>
    <tr>
        <td>
            <h1>Labas, užsisakei paslaugą!</h1>
            <p>{0},</p>
            <p>Jūsų užsakytos paslaugos informacija:</p>
            <p>
				Paslauga: {1}<br>
				Kaina: {2}<br>
				Atsiskaitymo būdas: {3}<br>
				Data ir laikas: {4} - {5}<br>
				Vieta: {6}<br>
				
			</p>
            <p>Jei turite klausimų, nedvejokite ir rašykite mums: <a href='mailto:4bartukai@gmail.com'>4bartukai@gmail.com</a></p>
            <br>
            <p>Nuoširdžiausi linkėjimai,</p>
            <p>Bartukai</p>
        </td>
    </tr>
</table>

</body>

</html>
";

            var user = _vcvsContext.User.FirstOrDefault(x => x.Id == request.UserId);
        
            if (user is null)
            {
                throw new Exception("Register email cannot be sent to user that doesn't exist");
            }
        
            var reservation = _vcvsContext.FullOrder
                .Include(x => x.Service)
                .Include(x => x.Room)
                .FirstOrDefault(x => x.OrderId == request.OrderId);

            if (reservation is null)
            {
                throw new Exception("Register email cannot be sent to user with not existing order");
            }

            var mailBody = string.Format(
                mailBodyTemplate,
                user.FirstName,
                reservation.Service.Name,
                $"{reservation.Service.Price} €",
                BuildPaymentTypeString(reservation),
                $"{reservation.DateFrom:yyyy-MM-dd HH:mm}",
                $"{reservation.DateTo:yyyy-MM-dd HH:mm}",
                BuildLocationString(reservation));

            var letter = new SendEmailRequest()
            {
                ReceiverEmail = user.Email,
                Subject = $"Paslauga \"{reservation.Service.Name}\" užsakyta",
                Body = mailBody
            };
        
            _emailService.SendEmail(letter);
        }
        catch (Exception e)
        {
            throw new Exception("Email failed");
        }
    }

    private string BuildLocationString(FullOrder reservation)
    {
        switch (reservation.Service.ServiceLocation)
        {
            case ServiceLocation.BusinessCenter:
                return $"Bartukų verslo centras, kambarys: {reservation.Room?.Name}";

            case ServiceLocation.OwnedBySpecialist:
                return $"{reservation.Service.Address} ({reservation.Service.AddressDescription})";
            
            case ServiceLocation.Online:
                return $"Nuotoliu: {reservation.Service.Link}";
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private string BuildPaymentTypeString(FullOrder reservation)
    {
        if (reservation.Order == null) throw new Exception("Order doesn't exist");
        switch (reservation.Order.PaymentType)
        {
            case PaymentType.Cash:
                return "Grynieji";
            case PaymentType.Card:
                return "Bankinė kortelė";
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}