using System;
using System.Collections.Generic;

namespace ConfygureOut
{
    public static class CombinedSourceUseExtension
    {
        public static BaseConfiguration UseCombinedSource(
            this BaseConfiguration configuration,
            string name,
            bool supportHotLoad,
            IEnumerable<BaseConfigurationSource> sources,
            TimeSpan? autoRefreshInterval = null
         )
        {
            var combinedSource = new CombinedSource(name, supportHotLoad, sources);
            return configuration.RegisterConfigurationSources((combinedSource, autoRefreshInterval));
        }
    }
}