using System.Linq;
using System;
using System.Configuration;

namespace ConfygureOut.Sources
{
    public class AppSettingsSource : BaseConfigurationSource
    {
        public AppSettingsSource(string name = "AppSettings") : base(name, supportsHotLoad: true) {}

        public override object GetConfigurationValue(string key, Type propertyType)
        {
            var appSettings = ConfigurationManager.AppSettings;
            return appSettings.AllKeys.Contains(key) ? appSettings[key] : null;
        }
    }
}
