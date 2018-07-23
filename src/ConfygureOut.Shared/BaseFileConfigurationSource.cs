using System;
using System.Collections.Generic;
using System.IO;

namespace ConfygureOut
{
    public abstract class BaseFileConfigurationSource: BaseConfigurationSource
    {
        protected readonly string ConfigFilePath;
        protected IDictionary<string, object> Configurations;
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly FileSystemWatcher _watcher;

        protected BaseFileConfigurationSource(string name, 
            string configFilePath,
            bool autoReloadOnFileChange = false) : base(name, supportsHotLoad: false)
        {
            ConfigFilePath = configFilePath;
            if (ConfigFilePath.IsHttpUrl() || !autoReloadOnFileChange)
            {
                return;
            }
            var directory = Path.GetDirectoryName(ConfigFilePath);
            var fileName = Path.GetFileName(ConfigFilePath);
            _watcher = new FileSystemWatcher
            {
                Path = directory,
                Filter = fileName,
                IncludeSubdirectories = false,
                NotifyFilter = NotifyFilters.Attributes |
                    NotifyFilters.CreationTime |
                    NotifyFilters.FileName |
                    NotifyFilters.LastAccess |
                    NotifyFilters.LastWrite |
                    NotifyFilters.Size |
                    NotifyFilters.Security,
                EnableRaisingEvents = true
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
        }

        public override object GetConfigurationValue(string key, Type valueType)
        {
            valueType = Nullable.GetUnderlyingType(valueType) ?? valueType;
            return Configurations.ContainsKey(key)
                ? Convert.ChangeType(Configurations[key], valueType)
              : ConfigurationValueNotFound.Instance;
        }
    }
}
