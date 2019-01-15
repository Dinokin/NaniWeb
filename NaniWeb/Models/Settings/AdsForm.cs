using System.ComponentModel.DataAnnotations;

namespace NaniWeb.Models.Settings
{
    public class AdsForm
    {
        [Required] public bool EnableAds { get; set; }

        public string AdsHeadScripts { get; set; }

        public string AdsBodyScripts { get; set; }
        
        public string AdsContainerCode { get; set; }
    }
}