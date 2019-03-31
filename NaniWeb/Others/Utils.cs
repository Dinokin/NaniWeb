using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace NaniWeb.Others
{
    public static class Utils
    {
        public static DirectoryInfo CurrentDirectory
        {
            get
            {
                var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);

                return new FileInfo(location.AbsolutePath.Replace("%20", " ")).Directory;
            }
        }

        public static bool IsInstalled()
        {
            return File.Exists($"{CurrentDirectory.FullName}{Path.DirectorySeparatorChar}installed.txt");
        }

        public static void ResizeImage(FileInfo origin, DirectoryInfo destination, string fileName, ushort width, ushort height)
        {
            using (var image = Image.Load(origin.OpenRead()))
            {
                image.Mutate(param => param.Resize(width, height));
                image.Save($"{destination.FullName}{Path.DirectorySeparatorChar}{fileName}{origin.Extension}");
            }
        }

        public static string GenerateSlug(string source)
        {
            var output = Regex.Replace(source, @"[^A-Za-z0-9\s]", string.Empty);
            return Regex.Replace(output, @"[\s]", "-").ToLower();
        }

        public static async Task BuildServiceWorker(string firebaseApiKey, string firebaseProjectId, string firebaseSenderId, IHostingEnvironment environment)
        {
            var serviceWorker = new StringBuilder();
            serviceWorker.AppendLine("importScripts('https://www.gstatic.com/firebasejs/5.9.2/firebase-app.js');");
            serviceWorker.AppendLine("importScripts('https://www.gstatic.com/firebasejs/5.9.2/firebase-messaging.js');");
            serviceWorker.AppendLine("var config = {");
            serviceWorker.AppendLine($"apiKey: '{firebaseApiKey}',");
            serviceWorker.AppendLine($"projectId: '{firebaseProjectId}',");
            serviceWorker.AppendLine($"messagingSenderId: '{firebaseSenderId}',");
            serviceWorker.AppendLine("};");
            serviceWorker.AppendLine("firebase.initializeApp(config);");
            serviceWorker.AppendLine("const messaging = firebase.messaging();");
            serviceWorker.AppendLine("messaging.setBackgroundMessageHandler(function(payload) {");
            serviceWorker.AppendLine("var title = payload.notification.title;");
            serviceWorker.AppendLine("var options = {");
            serviceWorker.AppendLine("body: payload.notification.body,");
            serviceWorker.AppendLine("icon: payload.data.icon,");
            serviceWorker.AppendLine("click: payload.fcmOptions.click");
            serviceWorker.AppendLine("}");
            serviceWorker.AppendLine("return self.registration.showNotification(title, options);");
            serviceWorker.AppendLine("});");
            serviceWorker.AppendLine("self.addEventListener('notificationclick', function(event) {");
            serviceWorker.AppendLine("event.notification.close();");
            serviceWorker.AppendLine("event.waitUntil(clients.openWindow(event.notification.click));");
            serviceWorker.AppendLine("});");

            await File.WriteAllTextAsync($"{environment.WebRootPath}{Path.DirectorySeparatorChar}firebase-messaging-sw.js", serviceWorker.ToString());
        }
    }
}