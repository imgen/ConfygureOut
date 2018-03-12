using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

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

        public async Task PushConfiguration(IConfiguration configuration)
        {
            await LoadConfigurations();
            var properties = configuration.GetConfigPropertiesBySourceName(Name);
            foreach (var property in properties.Where(x => x.CanWrite))
            {
                PushToProperty(configuration, property, property.GetCustomAttribute<ConfigurationSourceAttribute>());
            }
        }

        public virtual Task LoadConfigurations()
        {
            return Task.CompletedTask;
        }

        public virtual object PushToProperty(IConfiguration configuration,
            PropertyInfo property,
            ConfigurationSourceAttribute configSourceAttr)
        {
            var key = configSourceAttr.Key ?? property.Name;
            var value = GetConfigurationValue(key, property.DeclaringType);
            if (property.CanWrite)
            {
                property.SetValue(configuration, value);
            }

            return value;
        }

        public abstract object GetConfigurationValue(string key, Type propertyType);
    }
}
