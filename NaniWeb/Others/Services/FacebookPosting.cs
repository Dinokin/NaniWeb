using System.Net.Http;
using System.Threading.Tasks;

namespace NaniWeb.Others.Services
{
    public class FacebookPosting
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly SettingsKeeper _settingsKeeper;

        public FacebookPosting(IHttpClientFactory clientFactory, SettingsKeeper settingsKeeper)
        {
            _clientFactory = clientFactory;
            _settingsKeeper = settingsKeeper;
        }

        public async Task Post(string message)
        {
            if (bool.Parse(_settingsKeeper.GetSetting("EnableFacebookPosting").Value))
                using (var client = _clientFactory.CreateClient())
                {
                    var accessToken = _settingsKeeper.GetSetting("FacebookApiKey").Value;

                    await client.PostAsync($"https://graph.facebook.com/546349135390552/feed?message={message}&access_token={accessToken}", null);
                }
        }
    }
}