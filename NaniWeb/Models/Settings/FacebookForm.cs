using System.ComponentModel.DataAnnotations;

namespace NaniWeb.Models.Settings
{
    public class FacebookForm
    {
        [Required] public bool EnableFacebookPosting { get; set; }

        [Required] public string FacebookApiKey { get; set; }
    }
}