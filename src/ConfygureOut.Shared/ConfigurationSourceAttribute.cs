using System;

namespace ConfygureOut
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigurationSourceAttribute: Attribute
    {
        public string Name { get; set; }

        public string Key { get; set; }

        public ConfigurationSourceAttribute(string name, string key = null)
        {
            Name = name;
            Key = key;
        }
    }
}