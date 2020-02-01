// AlwaysTooLate.CVars (c) 2018-2019 Always Too Late.

using System;

namespace AlwaysTooLate.CVars
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ConfigClassAttribute : Attribute
    {
        public ConfigClassAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}