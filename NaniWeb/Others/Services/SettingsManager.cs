using System.Linq;
using NaniWeb.Data;

namespace NaniWeb.Others.Services
{
    public class SettingsManager
    {
        private readonly NaniWebContext _context;

        public SettingsManager(NaniWebContext naniWebContext) => _context = naniWebContext;
        
        public void AddSettings(string key, string value)
        {
            if (_context.Settings.Any(set => set.Name == key))
            {
                var setting = _context.Settings.Single(settings => settings.Name == key);
                setting.Value = value;
                _context.Settings.Update(setting);
            }
            else
            {
                var setting = new Setting
                {
                    Name = key,
                    Value = value
                }; 
                _context.Settings.Add(setting);
            }
            
            _context.SaveChanges();
        }

        public Setting GetSetting(string key)
        {
            Setting setting;
            
            if (_context.Settings.Any(set => set.Name == key)) 
                setting = _context.Settings.Single(settings => settings.Name == key);
            else 
                setting = new Setting 
                {
                    Name = key,
                    Value = string.Empty
                };

            return setting;
        }
    }
}