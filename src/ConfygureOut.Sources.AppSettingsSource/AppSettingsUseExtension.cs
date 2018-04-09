using System;

namespace ConfygureOut.Sources
{
    public static class AppSettingsUseExtension
    {
        public static BaseConfiguration UseAppSettings(
            this BaseConfiguration configuration,
            string sourceName = null,
            TimeSpan? autoRefreshInterval = null)
        {
            var appSettingsSource = new AppSettingsSource(sourceName);
            return configuration.RegisterConfigurationSources((appSettingsSource, autoRefreshInterval));
        }
    }
}
