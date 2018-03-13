using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TupleExtensions;

namespace ConfygureOut
{
    public class ConfigurationManager<TConfig>
        where TConfig : class, IConfiguration, new()
    {
        private readonly string _defaultSourceName;
        protected string DefaultSourceName => _defaultSourceName ?? _configurationSourceRegistration.Keys.First();

        private readonly Dictionary<string, ConfigurationSourceSetting> _configurationSourceRegistration =
            new Dictionary<string, ConfigurationSourceSetting>();

        public ConfigurationManager(string defaultSourceName = null)
        {
            _defaultSourceName = defaultSourceName;
        }

        public void RegisterConfigurationSources(params BaseConfigurationSource[] sources)
        {
            RegisterConfigurationSources(sources.Select(x => (x, (TimeSpan?)null)).ToArray());
        }

        public void RegisterConfigurationSources(params (BaseConfigurationSource source, TimeSpan? refreshInterval)[] sources)
        {
            foreach (var (source, refreshInterval) in sources)
            {
                _configurationSourceRegistration[source.Name] = new ConfigurationSourceSetting
                {
                    Source = source,
                    RefreshInterval = refreshInterval
                };
            }
        }

        public Task PullConfigurationsFromSource(BaseConfigurationSource source, IConfiguration target)
        {
            var properties = target.GetConfigPropertiesBySourceName(source.Name);
            var type = target.GetType();
            if (source.Name == DefaultSourceName)
            {
                properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Except(type.GetPropertiesWithAttribute<ConfigurationSourceAttribute>())
                    .Except(type.GetPropertiesWithAttribute<NonConfigurableAttribute>())
                    .PrependAll(properties.ToArray());
            }

            return source.PushConfiguration(target, properties);
        }

        public Task<TConfig> PullConfigurationsFromAllSources()
        {
            return PullConfigurationsFromAllSources(new TConfig());
        }

        public async Task<TConfig> PullConfigurationsFromAllSources(TConfig target)
        {
            foreach (var (_, configurationSourceSetting) in _configurationSourceRegistration)
            {
                await PullConfigurationsFromSource(configurationSourceSetting.Source, target)
                                                .ConfigureAwait(false);
            }

            return target;
        }

        public void StartAutoRefresh(TConfig target)
        {
            foreach(var (name, _) in _configurationSourceRegistration)
            {
                StartAutoRefresh(name, target);
            }
        }

        private async void AutoRefreshConfiguration(ConfigurationSourceSetting setting, TConfig target)
        {
            if (setting.RefreshInterval == null || setting.AutoRefreshState == AutoRefreshState.Stopped)
            {
                return;
            }

            await Task.Delay(setting.RefreshInterval.Value).ConfigureAwait(false);
            await PullConfigurationsFromSource(setting.Source, target).ConfigureAwait(false);
            AutoRefreshConfiguration(setting, target);
        }

        public object PullConfigurationValueFromSource(PropertyInfo property)
        {
            var configurationSourceAttr = property.GetCustomAttribute<ConfigurationSourceAttribute>();
            var sourceName = configurationSourceAttr?.Name?? DefaultSourceName;
            if (!_configurationSourceRegistration.ContainsKey(sourceName))
            {
                return null;
            }
            var source = _configurationSourceRegistration[sourceName].Source;
            var configurationKey = configurationSourceAttr?.Key?? property.Name;
            return !source.SupportsHotLoad ? null : 
                source.GetConfigurationValue(configurationKey, property.PropertyType);
        }

        public void StartAutoRefresh(string sourceName, TConfig target)
        {
            if (!_configurationSourceRegistration.ContainsKey(sourceName))
            {
                return;
            }

            var setting = _configurationSourceRegistration[sourceName];
            if (setting.AutoRefreshState == AutoRefreshState.Running)
            {
                return;
            }
            setting.AutoRefreshState = AutoRefreshState.Running;
            AutoRefreshConfiguration(setting, target);
        }

        public void StartAutoRefresh(BaseConfigurationSource source, TConfig target)
        {
            StartAutoRefresh(source.Name, target);
        }

        public void StopAutoRefresh()
        {
            foreach (var (name, _) in _configurationSourceRegistration)
            {
                StopAutoRefresh(name);
            }
        }

        public void StopAutoRefresh(string sourceName)
        {
            if (!_configurationSourceRegistration.ContainsKey(sourceName))
            {
                return;
            }

            var setting = _configurationSourceRegistration[sourceName];
            setting.AutoRefreshState = AutoRefreshState.Stopped;
        }

        public void StopAutoRefresh(BaseConfigurationSource source)
        {
            StopAutoRefresh(source.Name);
        }
    }
}