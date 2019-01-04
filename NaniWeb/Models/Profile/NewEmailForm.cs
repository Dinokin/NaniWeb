using System.ComponentModel.DataAnnotations;

namespace NaniWeb.Models.Profile
{
    public class NewEmailForm
    {
        [Required] [EmailAddress] public string NewEmail { get; set; }
    }
}