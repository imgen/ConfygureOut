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
            var configRSource = new ConfigRSource();
            var environmentVariableSource = new EnvironmentVariableSource("CONFYGURE_OUT_");
            var appSettingsSource = new AppSettingsSource();
            var configManager = new ConfigurationManager<MyConfig>();
            configManager.RegisterConfigurationSources(
                (configRSource, TimeSpan.FromSeconds(10)), 
                (environmentVariableSource, null),
                (appSettingsSource, null));
            
            var configuration = configManager.PullConfigurationsFromAllSources(new MyConfig(configManager)).Result;
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
        public MyConfig() { }

        public MyConfig(ConfigurationManager<MyConfig> manager)
        {
            Manager = manager;
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
