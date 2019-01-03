using System.ComponentModel.DataAnnotations;

namespace NaniWeb.Models.Settings
{
    public class DisqusForm
    {
        [Required] public bool EnableDisqus { get; set; }

        public string DisqusShortname { get; set; }
    }
}