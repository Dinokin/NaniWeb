using System.ComponentModel.DataAnnotations;

namespace NaniWeb.Models.Settings
{
    public class FacebookForm
    {
        [Required] public bool EnableFacebookPosting { get; set; }

        public string FacebookApiKey { get; set; }
    }
}