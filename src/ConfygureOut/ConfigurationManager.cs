using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using TupleExtensions;

namespace ConfygureOut
{
    public class ConfigurationManager<TConfig>
        where TConfig : class, IConfiguration, new()
    {
        private readonly Dictionary<string, ConfigurationSourceSetting> _configurationSourceRegistration =
            new Dictionary<string, ConfigurationSourceSetting>();

        public void RegisterConfigurationSources(params BaseConfigurationSource[] sources)
        {
            foreach (var source in sources)
            {
                _configurationSourceRegistration[source.Name] = new ConfigurationSourceSetting
                {
                    Source = source
                };
            }
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
            return source.PushConfiguration(target);
        }

        public Task<TConfig> PullConfigurationsFromAllSources()
        {
            return PullConfigurationsFromAllSources(new TConfig());
        }

        public async Task<TConfig> PullConfigurationsFromAllSources(TConfig target)
        {
            foreach (var (_, configurationSourceSetting) in _configurationSourceRegistration)
            {
                await configurationSourceSetting.Source.PushConfiguration(target);
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

            await Task.Delay(setting.RefreshInterval.Value);
            await PullConfigurationsFromSource(setting.Source, target);
        }

        public object PullConfigurationValueFromSource(PropertyInfo property)
        {
            var configurationSourceAttr = property.GetCustomAttribute<ConfigurationSourceAttribute>();
            var sourceName = configurationSourceAttr.Name;
            if (!_configurationSourceRegistration.ContainsKey(sourceName))
            {
                return null;
            }
            var source = _configurationSourceRegistration[sourceName].Source;
            return source.GetConfigurationValue(configurationSourceAttr.Key?? property.Name, property.PropertyType);
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