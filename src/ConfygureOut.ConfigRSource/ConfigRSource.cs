using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using ConfigR;

namespace ConfygureOut.Sources
{
    public class ConfigRSource: BaseConfigureSource
    {
        private readonly string _configFilePath;
        private IDictionary<string, object> _configurations;

        public ConfigRSource(string name, string configFilePath): base(name)
        {
            _configFilePath = configFilePath;
        }

        protected override async Task LoadConfigurations()
        {
            _configurations = await new Config().UseRoslynCSharpLoader(_configFilePath).LoadDictionary();
        }

        protected override void PushToProperty(IConfiguration configuration, PropertyInfo property, ConfigurationSourceAttribute configSourceAttr)
        {
            var key = configSourceAttr.Key ?? property.Name;
            var value = _configurations.ContainsKey(key)
                ? _configurations[key]
                : property.DeclaringType.GetDefaultValue();
            property.SetValue(configuration, value);
        }
    }
}
