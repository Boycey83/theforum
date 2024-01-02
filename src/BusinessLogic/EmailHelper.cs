using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using theforum.Model;
using theforum.Settings;

namespace theforum.BusinessLogic;

public class EmailHelper
{
    private readonly EmailSettings _emailSettings;

    public EmailHelper(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public void SendUserRegistrationEmail(string emailAddress, string username, int userId, Guid token)
    {
        var fromAddress = new MailAddress(_emailSettings.Address, _emailSettings.SenderName);
        var toAddress = new MailAddress(emailAddress, username);

        const string subject = "The Forum Registration: Please confirm new account";
        var body =
            $"Go to the following address to enable your new account: http" + 
            $"://{_emailSettings.ForumHostName}/UserAccount/{userId}/Confirm/{token}";
        var smtp = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(fromAddress.Address, _emailSettings.Password),
            Timeout = 20000
        };
        using var message = new MailMessage(fromAddress, toAddress) { Subject = subject, Body = body };
        smtp.Send(message);
    }

    public void SendUserPasswordResetEmail(UserAccount userAccount)
    {
        var fromAddress = new MailAddress(_emailSettings.Address, _emailSettings.SenderName);
        var toAddress = new MailAddress(userAccount.EmailAddress, userAccount.Username);
        const string subject = "The Forum: Password Reset";
        var body = $@"To reset your password please copy and paste the following code into the update password form:

{userAccount.ResetToken}

This code will expire after 24 hours.";
        var smtp = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(fromAddress.Address, _emailSettings.Password),
            Timeout = 20000
        };
        using var message = new MailMessage(fromAddress, toAddress) { Subject = subject, Body = body };
        smtp.Send(message);
    }
}