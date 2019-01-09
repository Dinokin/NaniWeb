using System.ComponentModel.DataAnnotations;

namespace NaniWeb.Models.Home
{
    public class Contact
    {
        [Required] public string Name { get; set; }

        [Required] [EmailAddress] public string Destination { get; set; }

        [Required] public string Content { get; set; }
    }
}