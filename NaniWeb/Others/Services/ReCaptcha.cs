using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace NaniWeb.Others.Services
{
    public class ReCaptcha
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly SettingsKeeper _settingsKeeper;

        public ReCaptcha(IHttpClientFactory clientFactory, SettingsKeeper settingsKeeper)
        {
            _clientFactory = clientFactory;
            _settingsKeeper = settingsKeeper;
        }

        public async Task<bool> ValidateResponse(string recaptchaResponse)
        {
            using (var client = _clientFactory.CreateClient())
            {
                var response = await client.GetStringAsync($"https://www.google.com/recaptcha/api/siteverify?secret={_settingsKeeper.GetSetting("RecaptchaSecretKey").Value}&response={recaptchaResponse}");
                dynamic jObject = JObject.Parse(response);

                return jObject.success == "true";
            }
        }
    }
}