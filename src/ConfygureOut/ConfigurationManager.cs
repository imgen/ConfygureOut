using System.Collections.Generic;
using TupleExtensions;

namespace ConfygureOut
{
    public class ConfigurationManager<T>
        where T : class, IConfiguration, new()
    {
        private readonly Dictionary<string, ConfigurationSourceSetting> _configurationSourceRegistration =
            new Dictionary<string, ConfigurationSourceSetting>();

        public void RegisterConfigurationSource(BaseConfigureSource source)
        {
            _configurationSourceRegistration[source.Name] = new ConfigurationSourceSetting
            {
                Source = source
            };
        }

        public void PullConfigurationsFromSource(BaseConfigureSource source, IConfiguration target)
        {
            source.PushConfiguration(target);
        }

        public void PullConfigurationsFromAllSources(T target)
        {
            foreach (var (_, configurationSourceSetting) in _configurationSourceRegistration)
            {
                configurationSourceSetting.Source.PushConfiguration(target);
            }
        }
    }
}