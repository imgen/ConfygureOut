using System;

namespace ConfygureOut
{
    public class ConfigurationSourceAttribute: Attribute
    {
        public string Name { get; set; }

        public string Key { get; set; }

        public ConfigurationSourceAttribute(string name, string key)
        {
            Name = name;
            Key = key;
        }
    }
}