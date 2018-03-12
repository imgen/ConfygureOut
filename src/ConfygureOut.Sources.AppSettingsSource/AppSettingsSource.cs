using System.Linq;
using System;

namespace ConfygureOut.Sources
{
    public class AppSettingsSource : BaseConfigurationSource
    {
        public AppSettingsSource(string name): base(name, supportsHotLoad: true) {}

        public override object GetConfigurationValue(string key, Type propertyType)
        {
            var appSettings = System.Configuration.ConfigurationManager.AppSettings;
            return appSettings.AllKeys.Contains(key) ? appSettings[key] : null;
        }
    }
}
