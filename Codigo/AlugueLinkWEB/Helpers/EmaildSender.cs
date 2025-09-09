using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;
using Core.Service;
using CoreEmailSender = Core.Service.IEmailSender;
using IdentityEmailSender = Microsoft.AspNetCore.Identity.UI.Services.IEmailSender;

namespace AlugueLinkWEB.Helpers
{
    public class EmailSender : CoreEmailSender, IdentityEmailSender
    {
        private readonly SmtpClient _client;
        private readonly string _from;
        private readonly string _webRootPath;

        public EmailSender(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _from = configuration["Smtp:From"]!;
            _webRootPath = environment.WebRootPath; // Obtém o caminho
            _client = new SmtpClient
            {
                Host = configuration["Smtp:Host"]!,
                Port = int.Parse(configuration["Smtp:Port"]!),
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                    configuration["Smtp:Username"]!,
                    configuration["Smtp:Password"]!)
            };
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_from),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);

            await _client.SendMailAsync(mailMessage);
        }
    }
}
