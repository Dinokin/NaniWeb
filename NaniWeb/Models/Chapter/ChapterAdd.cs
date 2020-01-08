using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace NaniWeb.Models.Chapter
{
    public class ChapterAdd
    {
        [Required] public int SeriesId { get; set; }
        public int? Volume { get; set; }

        [Required] public decimal ChapterNumber { get; set; }

        public string Name { get; set; }

        [Required] public IFormFile Pages { get; set; }
        
        [Required] public bool AnnounceOnDiscord { get; set; }

        [Required] public bool AnnounceOnReddit { get; set; }

        [Required] public bool RedditNsfw { get; set; }
    }
}