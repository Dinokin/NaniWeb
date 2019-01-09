using System.ComponentModel.DataAnnotations;

namespace NaniWeb.Models.Announcement
{
    public class AddAnnouncement
    {
        [Required] public string Title { get; set; }

        [Required] public string Content { get; set; }
    }
}