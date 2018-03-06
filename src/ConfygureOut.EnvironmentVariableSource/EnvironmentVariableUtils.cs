using System;
using System.Linq;

namespace ConfygureOut.EnvironmentVariableSource
{
    public static class EnvironmentVariableUtils
    {
        public static string GetEnvironmentVariable(this string key, string prefix = null)
        {
            key = key?.Prepend(prefix);
            return EnumUtils.GetValues<EnvironmentVariableTarget>()
                       .Select(x => Environment.GetEnvironmentVariable(key, x))
                       .FirstOrDefault(x => !x.IsNullOrEmpty()) ?? string.Empty;
        }
    }
}