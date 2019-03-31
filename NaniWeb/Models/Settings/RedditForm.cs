using System.ComponentModel.DataAnnotations;

namespace NaniWeb.Models.Settings
{
    public class RedditForm
    {
        [Required] public bool EnableReddit { get; set; }

        [Required] public string RedditUser { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string RedditPassword { get; set; }

        [Required] public string RedditClientId { get; set; }

        [Required] public string RedditClientSecret { get; set; }
    }
}