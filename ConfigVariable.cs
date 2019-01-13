// AlwaysTooLate.Core (c) 2018-2019 Always Too Late. All rights reserved.

using System;

namespace AlwaysTooLate.CVars
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ConfigVariable : Attribute
    {
        public ConfigVariable(string name, object defaultValue = null) { }
    }
}
