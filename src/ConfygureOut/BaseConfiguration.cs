using System.Reflection;
using System.Runtime.CompilerServices;

namespace ConfygureOut
{
    public class BaseConfiguration<TConfig>: IConfiguration
        where TConfig: class, IConfiguration, new()
    {
        public ConfigurationManager<TConfig> Manager { get; set; }

        public object PullConfigurationValueFromSource(PropertyInfo property)
        {
            return Manager.PullConfigurationValueFromSource(this, property);
        }

        public object PullConfigurationValueFromSource([CallerMemberName]string propertyName = null)
        {
            return PullConfigurationValueFromSource(GetType().GetProperty(propertyName));
        }
    }
}