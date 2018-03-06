using System.Reflection;
using System.Threading.Tasks;

namespace ConfygureOut
{
    public abstract class BaseConfigureSource
    {
        public string Name { get; set; }

        public void PushConfiguration(IConfiguration configuration)
        {
            LoadConfiguration();
            var properties = configuration.GetConfigPropertiesBySourceName(Name);
            foreach (var property in properties)
            {
                PushToProperty(configuration, property, property.GetCustomAttribute<ConfigurationSourceAttribute>());
            }
        }

        protected virtual Task LoadConfiguration()
        {
            return Task.CompletedTask;
        }

        protected abstract void PushToProperty(IConfiguration configuration, 
            PropertyInfo property, 
            ConfigurationSourceAttribute configSourceAttr);
    }
}
