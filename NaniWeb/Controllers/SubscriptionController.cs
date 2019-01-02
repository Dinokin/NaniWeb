using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NaniWeb.Others.Services;

namespace NaniWeb.Controllers
{
    [AllowAnonymous]
    public class SubscriptionController : Controller
    {
        private readonly FirebaseCloudMessaging _firebaseCloudMessaging;

        public SubscriptionController(FirebaseCloudMessaging firebaseCloudMessaging)
        {
            _firebaseCloudMessaging = firebaseCloudMessaging;
        }

        [HttpPost]
        public async Task Subscribe(string topic, string token)
        {
            await _firebaseCloudMessaging.Subscribe(topic, token);
        }

        [HttpPost]
        public async Task Unsubscribe(string topic, string token)
        {
            await _firebaseCloudMessaging.Unsubscribe(topic, token);
        }
    }
}