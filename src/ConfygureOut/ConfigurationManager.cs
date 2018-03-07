using System.Collections.Generic;
using System.Threading.Tasks;
using TupleExtensions;

namespace ConfygureOut
{
    public class ConfigurationManager<T>
        where T : class, IConfiguration, new()
    {
        private readonly Dictionary<string, ConfigurationSourceSetting> _configurationSourceRegistration =
            new Dictionary<string, ConfigurationSourceSetting>();

        public void RegisterConfigurationSources(params BaseConfigureSource[] sources)
        {
            foreach (var source in sources)
            {
                _configurationSourceRegistration[source.Name] = new ConfigurationSourceSetting
                {
                    Source = source
                };
            }
        }

        public Task PullConfigurationsFromSource(BaseConfigureSource source, IConfiguration target)
        {
            return source.PushConfiguration(target);
        }

        public Task<T> PullConfigurationsFromAllSources()
        {
            return PullConfigurationsFromAllSources(new T());
        }

        public async Task<T> PullConfigurationsFromAllSources(T target)
        {
            foreach (var (_, configurationSourceSetting) in _configurationSourceRegistration)
            {
                await configurationSourceSetting.Source.PushConfiguration(target);
            }

            return target;
        }
    }
}