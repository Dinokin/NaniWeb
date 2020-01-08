using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace NaniWeb.Others.Services
{
    public class ReCaptcha
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly SettingsManager _settingsManager;

        public ReCaptcha(IHttpClientFactory clientFactory, SettingsManager settingsManager)
        {
            _clientFactory = clientFactory;
            _settingsManager = settingsManager;
        }

        public async Task<bool> ValidateResponse(string recaptchaResponse)
        {
            using (var client = _clientFactory.CreateClient())
            {
                var response = await client.GetStringAsync($"https://www.google.com/recaptcha/api/siteverify?secret={_settingsManager.GetSetting("RecaptchaSecretKey").Value}&response={recaptchaResponse}");
                dynamic jObject = JObject.Parse(response);

                return jObject.success == "true";
            }
        }
    }
}