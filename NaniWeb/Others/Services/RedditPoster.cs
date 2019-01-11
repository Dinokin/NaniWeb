using System.Threading.Tasks;
using RedditSharp;

namespace NaniWeb.Others.Services
{
    public class RedditPoster
    {
        private readonly SettingsKeeper _settingsKeeper;
        private readonly Reddit _reddit;

        public RedditPoster(SettingsKeeper settingsKeeper)
        {
            _settingsKeeper = settingsKeeper;

            if (bool.Parse(_settingsKeeper.GetSetting("EnableReddit").Value))
            {
                var user = settingsKeeper.GetSetting("RedditUser").Value;
                var password = settingsKeeper.GetSetting("RedditPassword").Value;
                var clientId = settingsKeeper.GetSetting("RedditClientId").Value;
                var clientSecret = settingsKeeper.GetSetting("RedditClientSecret").Value;
                var redirectUrl = settingsKeeper.GetSetting("SiteUrl").Value;

                var botAgent = new BotWebAgent(user, password, clientId, clientSecret, redirectUrl);
                _reddit = new Reddit(botAgent);
            }
        }

        public async Task PostLink(string reddit, string title, string link)
        {
            if (bool.Parse(_settingsKeeper.GetSetting("EnableReddit").Value))
            {
                var subReddit = await _reddit.GetSubredditAsync(reddit);
                await subReddit.SubmitPostAsync(title, link);
            }
        }
    }
}