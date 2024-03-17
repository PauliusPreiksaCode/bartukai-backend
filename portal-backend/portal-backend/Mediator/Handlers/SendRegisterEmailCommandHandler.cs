using MediatR;
using portal_backend.Mediator.Commands;
using portal_backend.Models;
using portal_backend.Services;

namespace portal_backend.Mediator.Handlers;

public class SendRegisterEmailCommandHandler : IRequestHandler<SendRegisterEmailCommand>
{
    private readonly VcvsContext _vcvsContext;
    private readonly EmailService _emailService;
    
    public SendRegisterEmailCommandHandler(VcvsContext vcvsContext, EmailService emailService)
    {
        _vcvsContext = vcvsContext;
        _emailService = emailService;
    }
    
    public async Task Handle(SendRegisterEmailCommand request, CancellationToken cancellationToken)
    {
        var user = _vcvsContext.User.FirstOrDefault(x => x.Id == request.UserId);
        var mailBodyTemplate= @"
<!DOCTYPE html>
<html lang='lt'>

<body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>

<table style='max-width: 600px; margin: 0 auto; background-color: #fff; padding: 20px; border-radius: 8px;'>
    <tr>
        <td>
            <h1>Labas, čia sveikinasi Bartukų verslo centras!</h1>
            <p>{0},</p>
            <p>Džiaugiamės, jog pasirinkote mus!</p>
            <p>Jūsų registracija buvo pirmasis žingsnis leidžiantis pradėti naudotis mūsų verslo centro teikiamomis paslaugomis.</p>
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

        if (user is null)
        {
            throw new Exception("Register email cannot be sent to user that doesn't exist");
        }
        
        var mailBody = string.Format(mailBodyTemplate, user.FirstName);

        var letter = new SendEmailRequest()
        {
            ReceiverEmail = user.Email,
            Subject = "Sveikiname prisijungus prie Bartukų verslo centro!",
            Body = mailBody
        };
        
        _emailService.SendEmail(letter);
    }
}