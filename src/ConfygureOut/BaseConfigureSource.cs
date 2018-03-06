using System.Reflection;
using System.Threading.Tasks;

namespace ConfygureOut
{
    public abstract class BaseConfigureSource
    {
        public string Name { get; set; }

        protected BaseConfigureSource(string name)
        {
            Name = name;
        }

        public async Task PushConfiguration(IConfiguration configuration)
        {
            await LoadConfigurations();
            var properties = configuration.GetConfigPropertiesBySourceName(Name);
            foreach (var property in properties)
            {
                PushToProperty(configuration, property, property.GetCustomAttribute<ConfigurationSourceAttribute>());
            }
        }

        protected virtual Task LoadConfigurations()
        {
            return Task.CompletedTask;
        }

        protected abstract void PushToProperty(IConfiguration configuration, 
            PropertyInfo property, 
            ConfigurationSourceAttribute configSourceAttr);
    }
}
