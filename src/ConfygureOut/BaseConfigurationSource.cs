using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TupleExtensions;

namespace ConfygureOut
{
    public abstract class BaseConfigurationSource
    {
        public string Name { get; set; }

        public bool SupportsHotLoad { get; }

        protected BaseConfigurationSource(string name, bool supportsHotLoad)
        {
            Name = name;
            SupportsHotLoad = supportsHotLoad;
        }

        public async Task PushConfiguration(IConfiguration target, IEnumerable<PropertyInfo> properties)
        {
            await LoadConfigurations();
            var propertyArray = properties as PropertyInfo[] ?? properties.ToArray();
            PushConfiguration(target, propertyArray);
        }

        private void PushConfiguration(IConfiguration target, PropertyInfo[] properties)
        {
            if (!_targets.ContainsKey(target))
            {
                _targets[target] = properties;
            }

            foreach (var property in properties.Where(x => x.CanWrite))
            {
                PushToProperty(target, property, property.GetCustomAttribute<ConfigurationSourceAttribute>());
            }
        }

        public virtual Task LoadConfigurations()
        {
            return Task.CompletedTask;
        }

        public virtual object PushToProperty(IConfiguration target,
            PropertyInfo property,
            ConfigurationSourceAttribute configSourceAttr)
        {
            var key = configSourceAttr?.Key ?? property.Name;
            var value = GetConfigurationValue(key, property.DeclaringType);
            if (property.CanWrite)
            {
                property.SetValue(target, value);
            }

            return value;
        }

        public abstract object GetConfigurationValue(string key, Type propertyType);

        protected async Task PushToAllTargets()
        {
            await LoadConfigurations();
            foreach (var (target, properties) in _targets)
            {
                PushConfiguration(target, properties);
            }
        }

        private readonly Dictionary<IConfiguration, PropertyInfo[]> _targets = 
            new Dictionary<IConfiguration, PropertyInfo[]>();
    }
}
