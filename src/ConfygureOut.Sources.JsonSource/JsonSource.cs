using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ConfygureOut.Sources
{
    public class JsonSource : BaseFileConfigurationSource
    {
        public JsonSource(string configFilePath = null,
            bool autoReloadOnFileChange = false, 
            string name = null) : base(name ?? "Json", configFilePath, autoReloadOnFileChange)
        {
        }

        public override async Task LoadConfigurations()
        {
            string json;
            if (ConfigFilePath.IsHttpUrl())
            {
                json = await new HttpClient().GetStringAsync(ConfigFilePath);
            }
            else
            {
                json = File.ReadAllText(ConfigFilePath);
            }

            Configurations = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        }
    }
}
