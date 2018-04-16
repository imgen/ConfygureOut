using System.Threading.Tasks;
using ConfigR;

namespace ConfygureOut.Sources
{
    public class ConfigRSource: BaseFileConfigurationSource
    {
        public ConfigRSource(string configFilePath = null,
            bool autoReloadOnFileChange = false,
            string name = null): base(name?? "ConfigR", configFilePath?? "./config.csx", autoReloadOnFileChange)
        {
        }

        public override async Task LoadConfigurations()
        {
            Configurations = await new Config().UseRoslynCSharpLoader(ConfigFilePath).LoadDictionary();
        }
    }
}
