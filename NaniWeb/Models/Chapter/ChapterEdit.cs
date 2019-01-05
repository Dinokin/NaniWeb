using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace NaniWeb.Models.Chapter
{
    public class ChapterEdit
    {
        [Required] public int ChapterId { get; set; }
        
        public int? Volume { get; set; }

        [Required] public decimal ChapterNumber { get; set; }

        public string Name { get; set; }

        public IFormFile Pages { get; set; }
        
        public int? MangadexId { get; set; }
        
        [Required] public bool UploadToMangadex { get; set; }
    }
}