public interface IEmailService
{
    Task SendActivationEmailAsync(string toEmail, string activationToken, string iamUserId);
}
