using System.ComponentModel.DataAnnotations;

namespace NaniWeb.Models.Home
{
    public class ProjectSearch
    {
        public string Name { get; set; }

        [Required] public Data.Series.SeriesStatus Status { get; set; }
    }
}