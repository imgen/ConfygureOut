using System;
namespace ConfygureOut.Sources
{
    public static class ConfigRUseExtension
    {
        public static BaseConfiguration UseConfigR(
            this BaseConfiguration configuration,
            string configFilePath = null,
            bool autoReloadOnFileChange = false,
            string sourceName = null)
        {
            var configRSource = new ConfigRSource(configFilePath, 
                                                      autoReloadOnFileChange, 
                                                      sourceName);
            configuration.RegisterConfigurationSources(configRSource);
            return configuration;
        }
    }
}
