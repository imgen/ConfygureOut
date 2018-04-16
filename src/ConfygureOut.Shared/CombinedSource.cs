using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace ConfygureOut
{
    public class CombinedSource: BaseConfigurationSource
    {
        private readonly List<BaseConfigurationSource> _sources;
        public CombinedSource(string name, 
                              bool supportHotLoad, 
                              params BaseConfigurationSource[] sources)
            :base(name, supportHotLoad)
        {
            _sources = sources.ToList();
        }
        
        public CombinedSource(string name, 
            bool supportHotLoad, 
            IEnumerable<BaseConfigurationSource> sources)
            :this(name, supportHotLoad, sources.ToArray())
        {
        }

		public override async Task LoadConfigurations()
		{
            foreach(var source in _sources)
            {
                await source.LoadConfigurations();
            }
		}

		public override object GetConfigurationValue(string key, Type propertyType)
		{
            object value = null;
            foreach(var source in _sources)
            {
                value = source.GetConfigurationValue(key, propertyType);
                if (value != ConfigurationValueNotFound.Instance)
                {
                    break;
                }
            }

            return value;
		}
	}
}
