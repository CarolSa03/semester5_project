using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtp;

    public EmailService(IOptions<SmtpSettings> smtpOptions)
    {
        _smtp = smtpOptions.Value;
    }

    public async Task SendActivationEmailAsync(string toEmail, string activationToken, string iamUserId)
    {
        var activationUrl = $"http://localhost:5173/activate?token={activationToken}&iamUserId={iamUserId}";
        var subject = "Activate your Port Management account";
        var bodyHtml = $@"
            <p>Welcome! Please activate your account by clicking the button below:</p>
            <a href='{activationUrl}' style='display:inline-block;padding:10px 15px;border-radius:5px;background:#1976d2;color:#fff;text-decoration:none;'>Activate Account</a>
            <p>If the button doesn't work, copy and paste this link into your browser: <br>
            <a href='{activationUrl}'>{activationUrl}</a>
            </p>
        ";
        var bodyText = $"Welcome! Please activate your account by clicking the link: {activationUrl}";

        await SendEmailAsync(toEmail, subject, bodyText, bodyHtml);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string bodyText, string bodyHtml = null)
    {
        using var message = new MailMessage
        {
            From = new MailAddress(_smtp.SenderEmail, _smtp.SenderName),
            Subject = subject,
            Body = bodyText,
            IsBodyHtml = !string.IsNullOrWhiteSpace(bodyHtml)
        };

        message.To.Add(toEmail);

        if (!string.IsNullOrEmpty(bodyHtml))
        {
            var alternateView = AlternateView.CreateAlternateViewFromString(bodyHtml, null, "text/html");
            message.AlternateViews.Add(alternateView);
            var plainView = AlternateView.CreateAlternateViewFromString(bodyText, null, "text/plain");
            message.AlternateViews.Add(plainView);
        }

        using var smtp = new SmtpClient
        {
            Host = _smtp.Host,
            Port = _smtp.Port,
            EnableSsl = _smtp.EnableSsl,
            Credentials = new NetworkCredential(_smtp.Username, _smtp.Password)
        };

        try
        {
            await smtp.SendMailAsync(message);
        }
        catch (SmtpException ex)
        {
            throw new InvalidOperationException("Failed to send email. Check SMTP settings and credentials.", ex);
        }
    }
}
