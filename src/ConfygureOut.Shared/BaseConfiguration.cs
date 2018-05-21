using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ConfygureOut
{
    public class BaseConfiguration: IConfiguration
    {
        private string _defaultSourceName;

        protected static readonly BindingFlags PropertyBindingFlags = BindingFlags.NonPublic | BindingFlags.Public |
                                                                    BindingFlags.Instance;

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

        public BaseConfiguration RegisterConfigurationSources(params BaseConfigurationSource[] sources)
        {
            return RegisterConfigurationSources(sources.Select(x => (x, (TimeSpan?)null)).ToArray());
        }

        public BaseConfiguration RegisterConfigurationSources(params (BaseConfigurationSource source, TimeSpan? refreshInterval)[] sources)
        {
            foreach (var (source, refreshInterval) in sources)
            {
                _configurationSourceRegistration[source.Name] = new ConfigurationSourceSetting
                {
                    Source = source,
                    RefreshInterval = refreshInterval
                };
            }

            return this;
        }

        private static string GetCallingMemberName(int stackLevel = 1)
        {
            var callStackTrace = new StackTrace();
            var propertyFrame = callStackTrace.GetFrame(stackLevel); // 1: below GetPropertyName frame
            var properyAccessorName = propertyFrame.GetMethod().Name;

            return properyAccessorName.Replace("get_", "").Replace("set_", "");
        }

        protected object PullConfigurationValueFromSourceWithDefault(object defaultValue)
        {
            var property = GetType().GetProperty(
                GetCallingMemberName(stackLevel: 2), PropertyBindingFlags);
            return PullConfigurationValueFromSourceWithDefault(
                property, 
                // ReSharper disable once PossibleNullReferenceException
                property.PropertyType, defaultValue);
        }

        public object PullConfigurationValueFromSourceWithDefault(string propertyName, object defaultValue)
        {
            var property = GetType().GetProperty(propertyName, PropertyBindingFlags);
            return PullConfigurationValueFromSourceWithDefault(property, 
                // ReSharper disable once PossibleNullReferenceException
                property.PropertyType, defaultValue);
        }

        protected object PullConfigurationValueFromSource([CallerMemberName]string propertyName = null)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            return PullConfigurationValueFromSource(GetType().GetProperty(propertyName, PropertyBindingFlags));
        }

        protected T PullConfigurationValueFromSource<T>([CallerMemberName] string propertyName = null)
        {
            return (T)PullConfigurationValueFromSource(propertyName);
        }

        public T PullConfigurationValueFromSourceWithDefault<T>(string propertyName, T defaultValue)
        {
            return (T)PullConfigurationValueFromSourceWithDefault(
                GetType().GetProperty(propertyName, PropertyBindingFlags), typeof(T), defaultValue);
        }

        protected T PullConfigurationValueFromSourceWithDefault<T>(T defaultValue)
        {
            return (T)PullConfigurationValueFromSourceWithDefault(
                GetType().GetProperty(GetCallingMemberName(stackLevel: 2), PropertyBindingFlags), typeof(T), defaultValue);
        }

        public object PullConfigurationValueFromSource(PropertyInfo property)
        {
            return PullConfigurationValueFromSourceWithDefault(property, 
                property.PropertyType,
                ConfigurationValueNotFound.Instance);
        }

        public object PullConfigurationValueFromSourceWithDefault(
            PropertyInfo property, Type valueType, object defaultValue)
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

            var value = source.GetConfigurationValue(configurationKey, valueType);
            return value == ConfigurationValueNotFound.Instance? defaultReturnValue : value;
        }

        public Task PullConfigurationsFromSource(BaseConfigurationSource source)
        {
            var properties = this.GetConfigPropertiesBySourceName(source.Name);
            var type = GetType();
            if (source.Name == DefaultSourceName)
            {
                properties = type.GetProperties(PropertyBindingFlags)
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

        public BaseConfiguration StartAutoRefresh()
        {
            foreach(var (name, _) in _configurationSourceRegistration)
            {
                StartAutoRefresh(name);
            }
            return this;
        }

        private async Task AutoRefreshConfiguration(ConfigurationSourceSetting setting)
        {
            while (true)
            {
                if (setting.RefreshInterval == null || setting.AutoRefreshState == AutoRefreshState.Stopped)
                {
                    return;
                }

                await Task.Delay(setting.RefreshInterval.Value).ConfigureAwait(false);
                await PullConfigurationsFromSource(setting.Source).ConfigureAwait(false);
            }
        }

        public BaseConfiguration StartAutoRefresh(string sourceName)
        {
            if (!_configurationSourceRegistration.ContainsKey(sourceName))
            {
                return this;
            }

            var setting = _configurationSourceRegistration[sourceName];
            if (setting.AutoRefreshState == AutoRefreshState.Running)
            {
                return this;
            }
            setting.AutoRefreshState = AutoRefreshState.Running;
#pragma warning disable 4014
            AutoRefreshConfiguration(setting);
#pragma warning restore 4014
            return this;
        }

        public BaseConfiguration StartAutoRefresh(BaseConfigurationSource source)
        {
            return StartAutoRefresh(source.Name);
        }

        public BaseConfiguration StopAutoRefresh()
        {
            foreach (var (name, _) in _configurationSourceRegistration)
            {
                StopAutoRefresh(name);
            }

            return this;
        }

        public BaseConfiguration StopAutoRefresh(string sourceName)
        {
            if (!_configurationSourceRegistration.ContainsKey(sourceName))
            {
                return this;
            }

            var setting = _configurationSourceRegistration[sourceName];
            setting.AutoRefreshState = AutoRefreshState.Stopped;
            return this;
        }

        public BaseConfiguration StopAutoRefresh(BaseConfigurationSource source)
        {
            StopAutoRefresh(source.Name);
            return this;
        }
    }
}
