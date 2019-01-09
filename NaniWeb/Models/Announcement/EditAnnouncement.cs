using System.ComponentModel.DataAnnotations;

namespace NaniWeb.Models.Announcement
{
    public class EditAnnouncement
    {
        [Required] public int AnnouncementId { get; set; }

        [Required] public string Title { get; set; }

        [Required] public string Content { get; set; }
    }
}