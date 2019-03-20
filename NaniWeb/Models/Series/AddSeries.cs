using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace NaniWeb.Models.Series
{
    public class AddSeries
    {
        [Required] public string Name { get; set; }

        [Required] public string Author { get; set; }

        [Required] public string Artist { get; set; }

        [Required] public string Synopsis { get; set; }

        public int? MangadexId { get; set; }
        
        [Required] public bool DisplayMangadexLink { get; set; }

        [Required] public Data.Series.SeriesType Type { get; set; }

        [Required] public Data.Series.SeriesStatus Status { get; set; }

        [Required] public IFormFile Cover { get; set; }
    }
}