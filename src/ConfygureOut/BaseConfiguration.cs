using System.Reflection;
using System.Runtime.CompilerServices;

namespace ConfygureOut
{
    public class BaseConfiguration<TConfig>: IConfiguration
        where TConfig: class, IConfiguration, new()
    {
        [NonConfigurable]
        public ConfigurationManager<TConfig> Manager { get; set; }

        public object PullConfigurationValueFromSource(PropertyInfo property)
        {
            return Manager.PullConfigurationValueFromSource(property);
        }

        public object PullConfigurationValueFromSource([CallerMemberName]string propertyName = null)
        {
            return PullConfigurationValueFromSource(GetType().GetProperty(propertyName));
        }

        public T PullConfigurationValueFromSource<T>([CallerMemberName] string propertyName = null)
        {
            return (T) PullConfigurationValueFromSource(propertyName);
        }
    }
}