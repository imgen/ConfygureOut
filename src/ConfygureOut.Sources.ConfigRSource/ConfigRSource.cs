using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ConfigR;

namespace ConfygureOut.Sources
{
    public class ConfigRSource: BaseConfigurationSource
    {
        private readonly string _configFilePath;
        private IDictionary<string, object> _configurations;
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly FileSystemWatcher _watcher;

        public ConfigRSource(string configFilePath = "./config.csx",
            bool autoReloadOnFileChange = false,
            string name = "ConfigR"): base(name, supportsHotLoad: false)
        {
            _configFilePath = configFilePath;
            if (!_configFilePath.IsHttpUrl() || !autoReloadOnFileChange)
            {
                return;
            }
            var directory = Path.GetDirectoryName(configFilePath);
            var fileName = Path.GetFileName(configFilePath);
            _watcher = new FileSystemWatcher
            {
                Path = directory,
                Filter = fileName,
                IncludeSubdirectories = false,
                NotifyFilter = NotifyFilters.LastWrite
            };
            _watcher.Changed += async (sender, args) =>
            {
                _watcher.EnableRaisingEvents = false;
                try
                {
                    await PushToAllTargets();
                }
                finally
                {
                    _watcher.EnableRaisingEvents = true;
                }
            };
            _watcher.EnableRaisingEvents = true;
        }

        public override async Task LoadConfigurations()
        {
            _configurations = await new Config().UseRoslynCSharpLoader(_configFilePath).LoadDictionary();
        }

        public override object GetConfigurationValue(string key, Type propertyType)
        {
            return _configurations.ContainsKey(key)
                ? _configurations[key]
              : ConfigurationValueNotFound.Instance;
        }
    }
}
