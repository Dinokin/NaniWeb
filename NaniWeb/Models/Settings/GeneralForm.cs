using System.ComponentModel.DataAnnotations;

namespace NaniWeb.Models.Settings
{
    public class GeneralForm
    {
        [Required]
        public string SiteName { get; set; }

        [Required]
        public string SiteDescription { get; set; }

        [Required]
        public string SiteUrl { get; set; }
        
        [Required]
        public bool EnableRegistration { get; set; }
    }
}