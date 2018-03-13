using ConfygureOut.Sources;
using static System.Console;

namespace ConfygureOut.Examples
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new MyConfig();
            configuration.PullConfigurationsFromAllSources().Wait();
            WriteLine($"ApiUrl is {configuration.ApiUrl}");
            WriteLine($"MaxRetryTimes is {configuration.MaxRetryTimes}");
            WriteLine($"DbConnectionString is {configuration.DbConnectionString}");
            WriteLine($"DurationInHours is {configuration.DurationInHours}");
            WriteLine($"WhosWho is {configuration.WhosWho}");
            ReadKey();
        }
    }

    public class MyConfig: BaseConfiguration
    {
        public MyConfig(): base(defaultSourceName: nameof(ConfigSourceNames.ConfigR))
        {
            var configRSource = new ConfigRSource();
            var environmentVariableSource = new EnvironmentVariableSource("CONFYGURE_OUT_");
            var appSettingsSource = new AppSettingsSource();

            RegisterConfigurationSources(
                configRSource,
                environmentVariableSource,
                appSettingsSource);
        }

        public string ApiUrl { get; set; }
        public int MaxRetryTimes { get; set; }

        [ConfigurationSource(nameof(ConfigSourceNames.EnvironmentVariable),
            "DB_CONNECTION_STRING", IsSensitive = true)]
        public string DbConnectionString => PullConfigurationValueFromSource<string>();

        [ConfigurationSource(nameof(ConfigSourceNames.EnvironmentVariable),
            "DURATION_HOURS")]
        public int DurationInHours => PullConfigurationValueFromSource<int>();

        [ConfigurationSource(nameof(ConfigSourceNames.AppSettings))]
        public string WhosWho => PullConfigurationValueFromSource<string>();
    }

    public enum ConfigSourceNames
    {
        ConfigR,
        EnvironmentVariable,
        AppSettings
    }
}
