using CBMSystem.Models;

namespace CBMSystem.Repository.Service
{
    public class EmailService
    {
        private readonly CbmsContext _context;
        public EmailService(CbmsContext context)
        {
            _context = context;
        }

        //private readonly EmailSettings _settings;
        //public EmailService(IOptions<EmailSettings> options)
        //{
        //    _settings = options.Value;
        //}
        public void SendEmail(string toEmail, string subject, string body)
        {
            var settings = _context.EmailSettings.FirstOrDefault();
            if (settings == null)
                throw new Exception("Email settings not found.");

            var mail = new System.Net.Mail.MailMessage
            {
                From = new System.Net.Mail.MailAddress(settings.FromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mail.To.Add(toEmail);

            using var smtp = new System.Net.Mail.SmtpClient(settings.Host, settings.Port)
            {
                Credentials = new System.Net.NetworkCredential(settings.Username, settings.Password),
                EnableSsl = settings.EnableSsl
            };

            smtp.Send(mail);
        }
    }
}
