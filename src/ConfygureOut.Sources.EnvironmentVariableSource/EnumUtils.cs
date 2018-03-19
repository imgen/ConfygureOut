using System;

namespace ConfygureOut.Sources
{
    internal static class EnumUtils
    {
        public static TEnum[] GetValues<TEnum>()
            where TEnum : struct
        {
            return (TEnum[])Enum.GetValues(typeof(TEnum));
        }
    }
}