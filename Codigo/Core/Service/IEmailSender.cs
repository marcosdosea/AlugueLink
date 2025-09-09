using System.Threading.Tasks;

namespace Core.Service
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}
