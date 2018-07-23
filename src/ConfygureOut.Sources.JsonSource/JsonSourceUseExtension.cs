using System;

namespace ConfygureOut.Sources
{
    public static class JsonSourceUseExtension
    {
        public static BaseConfiguration UseJson(
            this BaseConfiguration configuration,
            string configFilePath = null,
            bool autoReloadOnFileChange = false,
            string sourceName = null,
            TimeSpan? autoRefreshInterval = null)
        {
            var jsonSource = new JsonSource(configFilePath,
                                                        autoReloadOnFileChange,
                                                        sourceName);
            return configuration.RegisterConfigurationSources((jsonSource, autoRefreshInterval));
        }
    }
}