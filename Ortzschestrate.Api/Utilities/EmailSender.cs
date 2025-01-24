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

        using (var client = new SmtpClient())
        {
            client.Connect("smtp.gmail.com", 465, true);

            // Note: only needed if the SMTP server requires authentication
            client.Authenticate(emailAddress, emailPassword);

            string res = await client.SendAsync(message);
            await client.DisconnectAsync(false);
            return res;
        }

        // Set up SMTP client
        // SmtpClient client = new SmtpClient("smtp.gmail.com", 465);
        // client.EnableSsl = true;
        // client.UseDefaultCredentials = false;
        // client.Credentials = new NetworkCredential(emailAddress, emailPassword);
        //
        // // Create email message
        // MailMessage mailMessage = new MailMessage();
        // mailMessage.From = new MailAddress(emailAddress, emailSender);
        // mailMessage.To.Add(toEmail);
        // mailMessage.Subject = subject;
        // mailMessage.IsBodyHtml = true;
        // mailMessage.Body = body;
        //
        // // Send email
        // client.Send(mailMessage);
    }
}