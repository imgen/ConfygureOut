using System;

namespace ConfygureOut
{
    public class ConfigurationSourceAttribute: Attribute
    {
        public string Name { get; set; }

        public string Key { get; set; }

        public bool IsSensitive { get; set; }

        public ConfigurationSourceAttribute(string name, string key = null, bool isSensitive = false)
        {
            Name = name;
            Key = key;
            IsSensitive = isSensitive;
        }
    }
}