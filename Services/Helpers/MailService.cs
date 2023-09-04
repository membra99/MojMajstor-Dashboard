using MimeKit.Text;
using MimeKit;
using Microsoft.Extensions.Options;
using MailKit.Security;
using MailKit.Net.Smtp;
using Universal.DTO.IDTO;
using Microsoft.AspNetCore.Http;

namespace Services.Helpers
{
    public class MailService
    {
        private readonly IOptions<EmailSettings> _emailConfig;
        public MailService(IOptions<EmailSettings> config) {
            _emailConfig = config;
        }
        public void SendEmail(EmailIDTO request)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_emailConfig.Value.EmailUserName));
            email.To.Add(MailboxAddress.Parse(request.To));
            email.Subject = request.Subject;
            email.Body = new TextPart(TextFormat.Html) { Text = request.Body };
            using var smtp = new SmtpClient();
            smtp.Connect(_emailConfig.Value.EmailHost, 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(_emailConfig.Value.EmailUserName, _emailConfig.Value.EmailUserPassword);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
