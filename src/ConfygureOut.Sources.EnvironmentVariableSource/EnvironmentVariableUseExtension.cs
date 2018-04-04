namespace ConfygureOut.Sources
{
    public static class EnvironmentVariableUseExtension
    {
        public static BaseConfiguration UseEnvironmentVariable(
            this BaseConfiguration configuration,
            string environmentVariableKeyPrefix = null,
            string sourceName = null)
        {
            var environmentVariableSource = new EnvironmentVariableSource(
                environmentVariableKeyPrefix, sourceName);
            return configuration.RegisterConfigurationSources(environmentVariableSource);
        }
    }
}
