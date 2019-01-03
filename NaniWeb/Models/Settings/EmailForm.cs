using System.ComponentModel.DataAnnotations;

namespace NaniWeb.Models.Settings
{
    public class EmailForm
    {
        [Required] public bool EnableRegistration { get; set; }

        [Required] public bool EnableEmailRecovery { get; set; }

        public string SmtpServer { get; set; }

        public string SmtpUser { get; set; }

        [DataType(DataType.Password)]
        public string SmtpPassword { get; set; }
    }
}