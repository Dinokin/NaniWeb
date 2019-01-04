using System.ComponentModel.DataAnnotations;

namespace NaniWeb.Models.Settings
{
    public class EmailForm
    {
        [Required] public bool EnableEmailRecovery { get; set; }

        [Required] public string SmtpServer { get; set; }

        [Required] public string SmtpUser { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string SmtpPassword { get; set; }
    }
}