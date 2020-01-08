using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace NaniWeb.Others.Services
{
    public class EmailSender
    {
        private readonly SettingsKeeper _settingsKeeper;
        private readonly SmtpClient _smtpClient;

        public EmailSender(SettingsKeeper settingsKeeper)
        {
            _settingsKeeper = settingsKeeper;

            if (bool.Parse(_settingsKeeper.GetSetting("EnableEmailRecovery").Value))
                _smtpClient = new SmtpClient(_settingsKeeper.GetSetting("SmtpServer").Value)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_settingsKeeper.GetSetting("SmtpUser").Value, _settingsKeeper.GetSetting("SmtpPassword").Value),
                    EnableSsl = true
                };
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if (bool.Parse(_settingsKeeper.GetSetting("EnableEmailRecovery").Value))
            {
                var message = new MailMessage
                {
                    From = new MailAddress($"noreply@{_settingsKeeper.GetSetting("SiteUrl").Value.Replace("https://", string.Empty)}"),
                    Body = htmlMessage,
                    Subject = subject
                };
                message.To.Add(email);

                await _smtpClient.SendMailAsync(message);
            }
        }
    }
}