using System.Net;
using System.Net.Mail;

namespace Service.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly string EMAIL = "filebox0x1@gmail.com";
        private readonly string PASSWORD = "uqfbhohviyvqegjd";
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(EMAIL, PASSWORD)
            };

            return client.SendMailAsync(new MailMessage(from: EMAIL, to: email, subject, message));
        }
    }
}
