using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NaniWeb.Data;

namespace NaniWeb.Others.Services
{
    public class SettingsKeeper
    {
        private readonly IServiceProvider _serviceProvider;

        public SettingsKeeper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Settings = new ConcurrentDictionary<string, string>();

            if (!Utils.IsInstalled())
                return;

            using (var scope = _serviceProvider.CreateScope())
            using (var context = scope.ServiceProvider.GetRequiredService<NaniWebContext>())
            {
                foreach (var setting in context.Settings.ToList())
                    Settings.AddOrUpdate(setting.Name, setting.Value, (oldKey, oldValue) => setting.Value);
            }
        }

        private ConcurrentDictionary<string, string> Settings { get; set; }

        public async Task AddSettings(string key, string value)
        {
            using (var scope = _serviceProvider.CreateScope())
            using (var context = scope.ServiceProvider.GetRequiredService<NaniWebContext>())
            {
                if (Settings.ContainsKey(key))
                {
                    var setting = await context.Settings.SingleAsync(settings => settings.Name == key);
                    setting.Value = value;
                    context.Settings.Update(setting);
                }
                else if (await context.Settings.SingleOrDefaultAsync(settings => settings.Name == key) is Setting outSetting)
                {
                    outSetting.Value = value;
                    context.Settings.Update(outSetting);
                }
                else
                {
                    var setting = new Setting
                    {
                        Name = key,
                        Value = value
                    };
                    await context.Settings.AddAsync(setting);
                }

                await context.SaveChangesAsync();
                Settings.AddOrUpdate(key, value, (oldKey, oldValue) => value);
            }
        }

        public Setting GetSetting(string key)
        {
            if (!Settings.TryGetValue(key, out var value))
                return null;

            var setting = new Setting
            {
                Name = key,
                Value = value
            };

            return setting;
        }

        public void SynchronizeSettings()
        {
            Settings = new ConcurrentDictionary<string, string>();

            using (var scope = _serviceProvider.CreateScope())
            using (var context = scope.ServiceProvider.GetRequiredService<NaniWebContext>())
            {
                foreach (var setting in context.Settings) Settings.AddOrUpdate(setting.Name, setting.Value, (oldKey, oldValue) => setting.Value);
            }
        }
    }
}