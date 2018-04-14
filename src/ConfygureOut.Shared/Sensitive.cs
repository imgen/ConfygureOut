using System;

namespace ConfygureOut
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SensitiveAttribute: Attribute
    {
    }
}