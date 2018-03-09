using System.Reflection;

namespace ConfygureOut.Sources
{
    public class EnvironmentVariableSource: BaseConfigurationSource
    {
        private readonly string _environmentVariableKeyPrefix;

        public EnvironmentVariableSource(string name, string environmentVariableKeyPrefix = null): base(name)
        {
            _environmentVariableKeyPrefix = environmentVariableKeyPrefix;
        }


        public override object PushToProperty(IConfiguration configuration, 
            PropertyInfo property, 
            ConfigurationSourceAttribute configSourceAttr)
        {
            var key = configSourceAttr.Key;
            if (key.IsNullOrEmpty())
            {
                key = property.Name;
            }

            var stringValue = key.GetEnvironmentVariable(_environmentVariableKeyPrefix);
            var type = property.DeclaringType;
            object value = stringValue;
            if (stringValue.IsNullOrEmpty())
            {
                value = type.GetDefaultValue();
            }
            else if (type == typeof(int))
            {
                value = int.Parse(stringValue);
            }
            else if (type == typeof(float))
            {
                value = float.Parse(stringValue);
            }
            else if(type == typeof(double))
            {
                value = double.Parse(stringValue);
            }
            else if (type == typeof(decimal))
            {
                value = decimal.Parse(stringValue);
            }

            if (property.CanWrite)
            {
                property.SetValue(configuration, value);
            }

            return value;
        }
    }
}
