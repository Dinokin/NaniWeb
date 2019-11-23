using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace NaniWeb.Others
{
    public class ManifestBuilder
    {
        [JsonProperty("short_name")] public string ShortName { get; set; }

        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("icons")] public Icon[] Icons { get; set; }

        [JsonProperty("theme_color")] public string ThemeColor { get; set; }

        [JsonProperty("background_color")] public string BackgroundColor { get; set; }

        [JsonProperty("gcm_sender_id")] public string GcmSenderId { get; set; }

        public async Task BuildManifest(IWebHostEnvironment environment)
        {
            var newManifest = JsonConvert.SerializeObject(this, Formatting.Indented);
            await File.WriteAllTextAsync($"{environment.WebRootPath}/manifest.json", newManifest);
        }

        public class Icon
        {
            [JsonProperty("src")] public string Src { get; set; }

            [JsonProperty("type")] public string Type { get; set; }

            [JsonProperty("sizes")] public string Sizes { get; set; }
        }
    }
}