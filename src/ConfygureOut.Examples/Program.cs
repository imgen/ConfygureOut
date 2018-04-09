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
            WriteLine($"Me is {configuration.Me}");
            ReadKey();
        }
    }

    public class MyConfig: BaseConfiguration
    {
        public MyConfig(): base(defaultSourceName: nameof(ConfigSourceNames.ConfigR))
        {
            this.UseAppSettings()
                .UseConfigR(autoReloadOnFileChange: true)
                .UseEnvironmentVariable("CONFYGURE_OUT_");
        }

        public string ApiUrl { get; set; }
        public int MaxRetryTimes { get; set; }

        [ConfigurationSource(nameof(ConfigSourceNames.EnvironmentVariable))]
        [ConfigurationKey("DB_CONNECTION_STRING")]
        [Sensitive]
        public string DbConnectionString => PullConfigurationValueFromSourceWithDefault("Not connected to DB");

        [ConfigurationSource(nameof(ConfigSourceNames.EnvironmentVariable),
            "DURATION_HOURS")]
        public int DurationInHours => PullConfigurationValueFromSourceWithDefault(4);

        [ConfigurationSource(nameof(ConfigSourceNames.AppSettings))]
        public string WhosWho => PullConfigurationValueFromSourceWithDefault("Me");

        [ConfigurationSource(nameof(ConfigSourceNames.AppSettings))]
        public string Me => PullConfigurationValueFromSourceWithDefault("I am me");
    }

    public enum ConfigSourceNames
    {
        ConfigR,
        EnvironmentVariable,
        AppSettings
    }
}
