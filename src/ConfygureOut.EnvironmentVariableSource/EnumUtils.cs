using System;

namespace ConfygureOut.EnvironmentVariableSource
{
    public static class EnumUtils
    {
        public static TEnum[] GetValues<TEnum>()
            where TEnum : struct
        {
            return (TEnum[])Enum.GetValues(typeof(TEnum));
        }
    }
}