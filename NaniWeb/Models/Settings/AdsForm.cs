using System.ComponentModel.DataAnnotations;

namespace NaniWeb.Models.Settings
{
    public class AdsForm
    {
        [Required] public bool EnableAds { get; set; }

        [Required] public string AdsHeaderCode { get; set; }

        [Required] public string AdsLocationTop { get; set; }

        [Required] public string AdsLocationMiddle { get; set; }

        [Required] public string AdsLocationBottom { get; set; }
    }
}