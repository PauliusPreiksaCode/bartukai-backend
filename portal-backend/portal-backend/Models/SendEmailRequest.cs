namespace portal_backend.Models;

public class SendEmailRequest
{
    public string ReceiverEmail { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}