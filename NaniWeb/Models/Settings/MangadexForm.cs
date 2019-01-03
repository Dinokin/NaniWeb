using System.ComponentModel.DataAnnotations;

namespace NaniWeb.Models.Settings
{
    public class MangadexForm
    {
        [Required] public bool EnableMangadexAutoUpload { get; set; }

        public string MangadexUser { get; set; }

        [DataType(DataType.Password)]
        public string MangadexPassword { get; set; }

        public int MangadexGroupId { get; set; }
    }
}