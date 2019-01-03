using System.ComponentModel.DataAnnotations;

namespace NaniWeb.Models.Settings
{
    public class DiscordForm
    {
        [Required] public bool EnableDiscordBot { get; set; }

        public string DiscordToken { get; set; }

        public ulong DiscordChannelId { get; set; }
    }
}