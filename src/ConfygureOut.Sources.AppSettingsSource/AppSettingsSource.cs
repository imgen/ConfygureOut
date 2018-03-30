using System.Linq;
using System;
using System.Configuration;

namespace ConfygureOut.Sources
{
    public class AppSettingsSource : BaseConfigurationSource
    {
        public AppSettingsSource(string name = null) : 
        base(name ?? "AppSettings", supportsHotLoad: true) {}

        public override object GetConfigurationValue(string key, Type propertyType)
        {
            var appSettings = ConfigurationManager.AppSettings;
            return appSettings.AllKeys.Contains(key) ? appSettings[key] : 
                              (object) ConfigurationValueNotFound.Instance;
        }
    }
}
