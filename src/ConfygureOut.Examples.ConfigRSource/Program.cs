using static System.Console;

namespace ConfygureOut.Examples.ConfigRSource
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var configRSource = new Sources.ConfigRSource("configr", "config.csx");
            var configManager = new ConfigurationManager<MyConfig>();
            configManager.RegisterConfigurationSource(configRSource);
            var configuration = configManager.PullConfigurationsFromAllSources().Result;
            WriteLine($"The ApiUrl is {configuration.ApiUrl}");
            WriteLine($"The MaxRetryTimes is {configuration.MaxRetryTimes}");
        }
    }

    public class MyConfig: IConfiguration
    {
        [ConfigurationSource("configr")]
        public string ApiUrl { get; set; }
        [ConfigurationSource("configr")]
        public int MaxRetryTimes { get; set; }
    }
}
