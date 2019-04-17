using System.ComponentModel.DataAnnotations;

namespace NaniWeb.Models.Settings
{
    public class AdsForm
    {
        [Required] public bool EnableAds { get; set; }

        public string AdsHeaderCode { get; set; }

        public string AdsLocationTop { get; set; }

        public string AdsLocationMiddle { get; set; }

        public string AdsLocationBottom { get; set; }
    }
}