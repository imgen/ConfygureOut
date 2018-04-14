using System;

namespace ConfygureOut
{
    internal class ConfigurationSourceSetting
    {
        public BaseConfigurationSource Source { get; set; }

        public TimeSpan? RefreshInterval { get; set; }

        public AutoRefreshState AutoRefreshState { get; set; } = AutoRefreshState.Stopped;
    }

    internal enum AutoRefreshState
    {
        Running,
        Stopped
    }
}