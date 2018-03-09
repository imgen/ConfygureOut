using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ConfygureOut
{
    public abstract class BaseConfigurationSource
    {
        public string Name { get; set; }

        protected BaseConfigurationSource(string name)
        {
            Name = name;
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

        public abstract object PushToProperty(IConfiguration configuration, 
            PropertyInfo property, 
            ConfigurationSourceAttribute configSourceAttr);
    }
}
