using MailKit.Net.Smtp;
using MimeKit;
using portal_backend.Models;

namespace portal_backend.Services;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }
    
    public void SendEmail(SendEmailRequest request)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_config["EmailConfig:Name"], _config["EmailConfig:SmtpServerUserName"]));
            message.To.Add(new MailboxAddress("", request.ReceiverEmail));
            message.Subject = request.Subject;
        
            var builder = new BodyBuilder
            {
                HtmlBody = request.Body
            };
            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
        
            client.Connect(_config["EmailConfig:SmtpServer"], int.Parse(_config["EmailConfig:SmtpServerPort"] ?? "0"), true);
            client.Authenticate(_config["EmailConfig:SmtpServerUserName"], _config["EmailConfig:SmtpServerUserPassword"]);
            
            client.Send(message);
            client.Disconnect(true);
        }
        catch (Exception e)
        {
            throw new Exception("Something with email");
        }
    }
}