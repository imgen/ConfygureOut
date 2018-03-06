using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConfygureOut
{
    public interface IConfiguration
    {
        
    }

    public static class ConfiurationExtensions
    {
        public static List<PropertyInfo> GetConfigPropertiesBySourceName(this IConfiguration target, string name)
        {
            return target.GetType().GetPropertiesWithAttribute<ConfigurationSourceAttribute>()
                .Where(prop => prop.GetCustomAttributes<ConfigurationSourceAttribute>()
                    .Any(attr => attr.Name == name)).ToList();
        }
    }
}