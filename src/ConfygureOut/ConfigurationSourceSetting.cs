using System;

namespace ConfygureOut
{
    public class ConfigurationSourceSetting
    {
        public TimeSpan? RefreshInterval { get; set; }
        public BaseConfigureSource Source { get; set; }
    }
}