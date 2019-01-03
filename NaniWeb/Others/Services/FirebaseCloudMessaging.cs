using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FcmSharp;
using FcmSharp.Requests;
using FcmSharp.Settings;

namespace NaniWeb.Others.Services
{
    public class FirebaseCloudMessaging : IDisposable
    {
        private readonly FcmClient _fcmClient;
        private readonly SettingsKeeper _settingsKeeper;

        public FirebaseCloudMessaging(SettingsKeeper settingsKeeper)
        {
            if (bool.Parse(settingsKeeper.GetSetting("EnableFcm").Value))
                return;

            _settingsKeeper = settingsKeeper;
            var settings = FileBasedFcmClientSettings.CreateFromFile($"{Utils.CurrentDirectory.FullName}{Path.DirectorySeparatorChar}fcmkey.json");
            _fcmClient = new FcmClient(settings);
        }

        public void Dispose()
        {
            _fcmClient?.Dispose();
        }

        public async Task SendNotification(string title, string body, string link, string icon, string topic)
        {
            if (bool.Parse(_settingsKeeper.GetSetting("EnableFcm").Value))
            {
                var notification = new Notification
                {
                    Title = title,
                    Body = body
                };

                var fcmMessage = new FcmMessage
                {
                    Message = new Message
                    {
                        Notification = notification,
                        Topic = topic,
                        WebpushConfig = new WebpushConfig
                        {
                            Headers = new Dictionary<string, string>
                            {
                                {"urgency-option", "high"}
                            },
                            FcmOptions = new WebpushFcmOptions
                            {
                                Link = link
                            }
                        },
                        Data = new Dictionary<string, string>
                        {
                            {"icon", icon}
                        }
                    }
                };

                await _fcmClient.SendAsync(fcmMessage);
            }
        }

        public async Task Subscribe(string topic, string token)
        {
            if (bool.Parse(_settingsKeeper.GetSetting("EnableFcm").Value))
            {
                var request = new TopicManagementRequest
                {
                    RegistrationTokens = new[] {token},
                    Topic = topic
                };

                await _fcmClient.SubscribeToTopic(request);
            }
        }

        public async Task Unsubscribe(string topic, string token)
        {
            if (bool.Parse(_settingsKeeper.GetSetting("EnableFcm").Value))
            {
                var request = new TopicManagementRequest
                {
                    RegistrationTokens = new[] {token},
                    Topic = topic
                };

                await _fcmClient.UnsubscribeFromTopic(request);
            }
        }
    }
}