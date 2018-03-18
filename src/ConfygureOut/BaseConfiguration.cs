using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TupleExtensions;

namespace ConfygureOut
{
    public class BaseConfiguration: IConfiguration
    {
        private string _defaultSourceName;

        protected string DefaultSourceName
        {
            get => _defaultSourceName ?? _configurationSourceRegistration.Keys.First();
            set => _defaultSourceName = value;
        }

        private readonly Dictionary<string, ConfigurationSourceSetting> _configurationSourceRegistration =
            new Dictionary<string, ConfigurationSourceSetting>();

        public BaseConfiguration(string defaultSourceName = null)
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

        private string GetCallingMemberName(int stackLevel = 1)
        {
            var callStackTrace = new StackTrace();
            var propertyFrame = callStackTrace.GetFrame(stackLevel); // 1: below GetPropertyName frame
            string properyAccessorName = propertyFrame.GetMethod().Name;

            return properyAccessorName.Replace("get_", "").Replace("set_", "");
        }

        protected object PullConfigurationValueFromSourceWithDefault(object defaultValue)
        {
            return PullConfigurationValueFromSourceWithDefault(
                GetType().GetProperty(GetCallingMemberName(stackLevel: 2)), defaultValue);
        }

        public object PullConfigurationValueFromSourceWithDefault(string propertyName, object defaultValue)
        {
            return PullConfigurationValueFromSourceWithDefault(
                GetType().GetProperty(propertyName), defaultValue);
        }

        protected object PullConfigurationValueFromSource([CallerMemberName]string propertyName = null)
        {
            return PullConfigurationValueFromSource(GetType().GetProperty(propertyName));
        }

        protected T PullConfigurationValueFromSource<T>([CallerMemberName] string propertyName = null)
        {
            return (T)PullConfigurationValueFromSource(propertyName);
        }

        public T PullConfigurationValueFromSourceWithDefault<T>(string propertyName, T defaultValue)
        {
            return (T)PullConfigurationValueFromSourceWithDefault(
                GetType().GetProperty(propertyName), defaultValue);
        }

        protected T PullConfigurationValueFromSourceWithDefault<T>(T defaultValue)
        {
            return (T)PullConfigurationValueFromSourceWithDefault(
                GetType().GetProperty(GetCallingMemberName(stackLevel: 2)), defaultValue);
        }

        public object PullConfigurationValueFromSource(PropertyInfo property)
        {
            return PullConfigurationValueFromSourceWithDefault(property, ConfigurationValueNotFound.Instance);
        }

        public object PullConfigurationValueFromSourceWithDefault(PropertyInfo property, object defaultValue)
        {
            var configurationSourceAttr = property.GetCustomAttribute<ConfigurationSourceAttribute>();
            var sourceName = configurationSourceAttr?.Name ?? DefaultSourceName;
            var defaultReturnValue = defaultValue ?? ConfigurationValueNotFound.Instance;
            if (!_configurationSourceRegistration.ContainsKey(sourceName))
            {
                return defaultReturnValue;
            }
            var source = _configurationSourceRegistration[sourceName].Source;
            var configurationKey = configurationSourceAttr?.Key ?? property.Name;
            if (!source.SupportsHotLoad)
            {
                return defaultReturnValue;
            }

            var value = source.GetConfigurationValue(configurationKey, property.PropertyType);
            return value == ConfigurationValueNotFound.Instance? defaultReturnValue : value;
        }

        public Task PullConfigurationsFromSource(BaseConfigurationSource source)
        {
            var properties = this.GetConfigPropertiesBySourceName(source.Name);
            var type = GetType();
            if (source.Name == DefaultSourceName)
            {
                properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Except(type.GetPropertiesWithAttribute<ConfigurationSourceAttribute>())
                    .Except(type.GetPropertiesWithAttribute<NonConfigurableAttribute>())
                    .PrependAll(properties.ToArray());
            }

            return source.PushConfiguration(this, properties);
        }

        public async Task PullConfigurationsFromAllSources()
        {
            foreach (var (_, configurationSourceSetting) in _configurationSourceRegistration)
            {
                await PullConfigurationsFromSource(configurationSourceSetting.Source)
                                                .ConfigureAwait(false);
            }
        }

        public void StartAutoRefresh()
        {
            foreach(var (name, _) in _configurationSourceRegistration)
            {
                StartAutoRefresh(name);
            }
        }

        private async void AutoRefreshConfiguration(ConfigurationSourceSetting setting)
        {
            if (setting.RefreshInterval == null || setting.AutoRefreshState == AutoRefreshState.Stopped)
            {
                return;
            }

            await Task.Delay(setting.RefreshInterval.Value).ConfigureAwait(false);
            await PullConfigurationsFromSource(setting.Source).ConfigureAwait(false);
            AutoRefreshConfiguration(setting);
        }

        public void StartAutoRefresh(string sourceName)
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
            AutoRefreshConfiguration(setting);
        }

        public void StartAutoRefresh(BaseConfigurationSource source)
        {
            StartAutoRefresh(source.Name);
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