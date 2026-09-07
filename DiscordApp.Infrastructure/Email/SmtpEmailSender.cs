using System.Net;
using System.Net.Mail;
using DiscordApp.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DiscordApp.Infrastructure.Email;

public class SmtpEmailSender(IConfiguration configuration) : IEmailSender
{
    public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        var smtpHost = configuration["EmailSettings:SmtpHost"];
        var smtpPort = int.Parse(configuration["EmailSettings:SmtpPort"]!);
        var smtpUser = configuration["EmailSettings:Username"];
        var smtpPass = configuration["EmailSettings:Password"];
        var fromEmail = configuration["EmailSettings:FromEmail"];

        using var client = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUser, smtpPass),
            EnableSsl = true
        };

        var message = new MailMessage(fromEmail!, toEmail, subject, htmlBody)
        {
            IsBodyHtml = true
        };

        await client.SendMailAsync(message);
    }
}