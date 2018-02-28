using System.Reflection;

namespace ConfygureOut
{
    public abstract class BaseConfigureSource
    {
        public string Name { get; set; }

        public void PushConfiguration(BaseConfiguration configuration)
        {
            var properties = configuration.GetConfigPropertiesBySourceName(Name);
            foreach (var property in properties)
            {
                PushToProperty(configuration, property);
            }
        }

        protected abstract void PushToProperty(BaseConfiguration configuration, PropertyInfo property);
    }
}
