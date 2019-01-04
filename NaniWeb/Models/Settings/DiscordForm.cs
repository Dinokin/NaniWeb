using System.ComponentModel.DataAnnotations;

namespace NaniWeb.Models.Settings
{
    public class DiscordForm
    {
        [Required] public bool EnableDiscordBot { get; set; }

        [Required] public string DiscordToken { get; set; }

        [Required] public ulong DiscordChannelId { get; set; }
    }
}