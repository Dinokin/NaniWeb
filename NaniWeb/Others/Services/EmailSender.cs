using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace NaniWeb.Others.Services
{
    public class EmailSender
    {
        private readonly SettingsManager _settingsManager;
        private readonly SmtpClient _smtpClient;

        public EmailSender(SettingsManager settingsManager)
        {
            _settingsManager = settingsManager;

            if (bool.Parse(_settingsManager.GetSetting("EnableEmailRecovery").Value))
                _smtpClient = new SmtpClient(_settingsManager.GetSetting("SmtpServer").Value)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_settingsManager.GetSetting("SmtpUser").Value, _settingsManager.GetSetting("SmtpPassword").Value),
                    EnableSsl = true
                };
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if (bool.Parse(_settingsManager.GetSetting("EnableEmailRecovery").Value))
            {
                var message = new MailMessage
                {
                    From = new MailAddress($"noreply@{_settingsManager.GetSetting("SiteUrl").Value.Replace("https://", string.Empty)}"),
                    Body = htmlMessage,
                    Subject = subject
                };
                message.To.Add(email);

                await _smtpClient.SendMailAsync(message);
            }
        }
    }
}