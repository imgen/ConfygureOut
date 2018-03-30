using System;
namespace ConfygureOut.Sources
{
    public static class AppSettingsUseExtension
    {
        public static BaseConfiguration UseAppSettings(
            this BaseConfiguration configuration,
            string sourceName = null)
        {
            var appSettingsSource = new AppSettingsSource(sourceName);
            configuration.RegisterConfigurationSources(appSettingsSource);
            return configuration;
        }
    }
}
