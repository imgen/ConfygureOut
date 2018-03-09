using ConfygureOut.Sources;
using static System.Console;

namespace ConfygureOut.Examples
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var configRSource = new ConfigRSource(nameof(ConfigSourceNames.ConfigR), "config.csx");
            var environmentVariableSource = new EnvironmentVariableSource(nameof(ConfigSourceNames.EnvironmentVariable),
                "CONFYGURE_OUT_");
            var configManager = new ConfigurationManager<MyConfig>();
            configManager.RegisterConfigurationSources(configRSource, environmentVariableSource);
            
            var configuration = configManager.PullConfigurationsFromAllSources().Result;
            WriteLine($"The ApiUrl is {configuration.ApiUrl}");
            WriteLine($"The MaxRetryTimes is {configuration.MaxRetryTimes}");
        }
    }

    public class MyConfig: BaseConfiguration<MyConfig>
    {
        [ConfigurationSource(nameof(ConfigSourceNames.ConfigR))]
        public string ApiUrl { get; set; }
        [ConfigurationSource(nameof(ConfigSourceNames.ConfigR))]
        public int MaxRetryTimes { get; set; }

        [ConfigurationSource(nameof(ConfigSourceNames.EnvironmentVariable),
            "DB_CONNECTION_STRING", IsSensitive = true)]
        public string DbConnectionString => (string) PullConfigurationValueFromSource();
    }

    public enum ConfigSourceNames
    {
        ConfigR,
        EnvironmentVariable
    }
}
