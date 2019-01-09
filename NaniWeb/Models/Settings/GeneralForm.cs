using System.ComponentModel.DataAnnotations;

namespace NaniWeb.Models.Settings
{
    public class GeneralForm
    {
        [Required] public bool EnableRegistration { get; set; }
        [Required] public string SiteName { get; set; }

        [Required] public string SiteDescription { get; set; }

        [Required] [Url] public string SiteUrl { get; set; }

        [Required] [EmailAddress] public string SiteEmail { get; set; }

        [Required] public string RecaptchaSiteKey { get; set; }

        [Required] public string RecaptchaSecretKey { get; set; }
        [Required] public string SiteFooter { get; set; }

        [Required] public string SiteSideBar { get; set; }

        [Required] public string SiteAboutPage { get; set; }
    }
}