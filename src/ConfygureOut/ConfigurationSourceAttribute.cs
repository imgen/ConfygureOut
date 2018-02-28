using System;

namespace ConfygureOut
{
    public class ConfigurationSourceAttribute: Attribute
    {
        public string Name { get; set; }

        public ConfigurationSourceAttribute(string name)
        {
            Name = name;
        }
    }
}