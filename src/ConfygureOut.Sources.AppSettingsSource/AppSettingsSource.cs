using System.Linq;
using System;

namespace ConfygureOut.Sources
{
    public class AppSettingsSource : BaseConfigurationSource
    {
        public override bool SupportsHotLoad => true;

		public AppSettingsSource(string name): base(name) {}

        public override object GetConfigurationValue(string key, Type propertyType)
        {
            var appSettings = System.Configuration.ConfigurationManager.AppSettings;
            return appSettings.AllKeys.Contains(key) ? appSettings[key] : null;
        }
    }
}
