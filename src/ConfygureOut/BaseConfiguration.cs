using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TupleExtensions;

namespace ConfygureOut
{
    public class BaseConfiguration
    {
        private readonly Dictionary<string, ConfigurationSourceSetting> _configurationSourceRegistration = new Dictionary<string, ConfigurationSourceSetting>();

        public List<PropertyInfo> GetConfigPropertiesBySourceName(string name)
        {
            return GetType().GetPropertiesWithAttribute<ConfigurationSourceAttribute>()
                .Where(prop => prop.GetCustomAttributes<ConfigurationSourceAttribute>()
                    .Any(attr => attr.Name == name)).ToList();
        }

        public void RegisterConfigurationSource(BaseConfigureSource source, TimeSpan? refreshInterval = null)
        {
            _configurationSourceRegistration[source.Name] = new ConfigurationSourceSetting
            {
                Source = source,
                RefreshInterval = refreshInterval
            };
        }

        public void PullConfigurationsFromSource(BaseConfigureSource source)
        {

        }

        public void PullConfigurationsFromAllSources()
        {
            foreach (var (_, configurationSourceSetting) in _configurationSourceRegistration)
            {
                configurationSourceSetting.Source.PushConfiguration(this);
            }
        }

        public void StartAutoRefresh()
        {

        }

        public void StopAutoRefresh()
        {

        }
    }
}