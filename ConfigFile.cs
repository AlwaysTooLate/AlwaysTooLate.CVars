// AlwaysTooLate.Core (c) 2018-2019 Always Too Late. All rights reserved.

using System;

namespace AlwaysTooLate.CVars
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class ConfigFile : Attribute
    {
        public ConfigFile(string name)
        {
        }
    }

    public class ConfigFile<TClass>
    {
        public static TClass Current { get; private set; }
    }
}
