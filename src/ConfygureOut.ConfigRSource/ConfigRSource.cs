using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConfigR;

namespace ConfygureOut.Sources
{
    public class ConfigRSource: BaseConfigurationSource
    {
        private readonly string _configFilePath;
        private IDictionary<string, object> _configurations;

        public ConfigRSource(string name, string configFilePath): base(name)
        {
            _configFilePath = configFilePath;
        }

        public override async Task LoadConfigurations()
        {
            _configurations = await new Config().UseRoslynCSharpLoader(_configFilePath).LoadDictionary();
        }

        public override object GetConfigurationValue(string key, Type propertyType)
        {
            return _configurations.ContainsKey(key)
                ? _configurations[key]
                : propertyType.GetDefaultValue();
        }
    }
}
