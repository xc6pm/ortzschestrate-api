using System.Net;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using Ortzschestrate.Infrastructure;

namespace Ortzschestrate.Api.Utilities;

public class EmailSender
{
    public Task<string> SendWalletVerificationEmailAsync(string toEmail, string verificationLink)
    {
        var body = $"""<a href="{verificationLink}">Click here to verify your wallet.</a>""";
        return SendEmailAsync(toEmail, "Verify your wallet", body);
    }

    public async Task<string> SendEmailAsync(string toEmail, string subject, string body)
    {
        var emailAddress = Environment.GetEnvironmentVariable(EnvKeys.MailAddress);
        var emailPassword = Environment.GetEnvironmentVariable(EnvKeys.MailPassword);
        var emailSender = Environment.GetEnvironmentVariable(EnvKeys.MailSender);


        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(emailSender, emailAddress));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = subject;

        message.Body = new TextPart(TextFormat.Html)
        {
            Text = body
        };

        using var client = new SmtpClient();
        await client.ConnectAsync("out.dnsexit.com", 587, false);

        // Note: only needed if the SMTP server requires authentication
        await client.AuthenticateAsync(new NetworkCredential(emailAddress, emailPassword));

        string res = await client.SendAsync(message);
        await client.DisconnectAsync(false);
        return res;
    }
}