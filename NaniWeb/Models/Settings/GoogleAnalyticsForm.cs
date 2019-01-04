using System.ComponentModel.DataAnnotations;

namespace NaniWeb.Models.Settings
{
    public class GoogleAnalyticsForm
    {
        [Required] public bool EnableGoogleAnalytics { get; set; }

        [Required] public string GoogleAnalyticsTrackingCode { get; set; }
    }
}