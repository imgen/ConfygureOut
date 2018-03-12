using System;
using ConfygureOut.Sources;
using static System.Console;
using System.Threading;

namespace ConfygureOut.Examples
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var configRSource = new ConfigRSource(nameof(ConfigSourceNames.ConfigR), "config.csx");
            var environmentVariableSource = new EnvironmentVariableSource(nameof(ConfigSourceNames.EnvironmentVariable),
                "CONFYGURE_OUT_");
            var appSettingsSource = new AppSettingsSource(nameof(ConfigSourceNames.AppSettings));
            var configManager = new ConfigurationManager<MyConfig>();
            configManager.RegisterConfigurationSources(
                (configRSource, TimeSpan.FromSeconds(10)), 
                (environmentVariableSource, null),
                (appSettingsSource, null));
            
            var configuration = configManager.PullConfigurationsFromAllSources().Result;
            configuration.Manager = configManager;
            WriteLine($"ApiUrl is {configuration.ApiUrl}");
            WriteLine($"MaxRetryTimes is {configuration.MaxRetryTimes}");
            WriteLine($"DbConnectionString is {configuration.DbConnectionString}");
            WriteLine($"DurationInHours is {configuration.DurationInHours}");
            WriteLine($"WhosWho is {configuration.WhosWho}");

            configManager.StartAutoRefresh(configuration);

            Thread.Sleep(TimeSpan.FromSeconds(30));

            configManager.StopAutoRefresh();

            Thread.Sleep(TimeSpan.FromSeconds(30));
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
