using System;

namespace ConfygureOut.Sources
{
    public class EnvironmentVariableSource: BaseConfigurationSource
    {
        private readonly string _environmentVariableKeyPrefix;

        public EnvironmentVariableSource(string name, 
             string environmentVariableKeyPrefix = null)
            : base(name, supportsHotLoad: true)
        {
            _environmentVariableKeyPrefix = environmentVariableKeyPrefix;
        }


        public override object GetConfigurationValue(string key, 
            Type propertyType)
        {
            var stringValue = key.GetEnvironmentVariable(_environmentVariableKeyPrefix);
            object value = stringValue;
            if (stringValue.IsNullOrEmpty())
            {
                value = propertyType.GetDefaultValue();
            }
            else if (propertyType == typeof(int))
            {
                value = int.Parse(stringValue);
            }
            else if (propertyType == typeof(float))
            {
                value = float.Parse(stringValue);
            }
            else if (propertyType == typeof(double))
            {
                value = double.Parse(stringValue);
            }
            else if (propertyType == typeof(decimal))
            {
                value = decimal.Parse(stringValue);
            }

            return value;
        }
    }
}
