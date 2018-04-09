using System;

namespace ConfygureOut
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigurationKeyAttribute: Attribute
    {
        public string Key { get; set; }

        public ConfigurationKeyAttribute(string key)
        {
            Key = key;
        }
    }
}