// AlwaysTooLate.Core (c) 2018-2019 Always Too Late. All rights reserved.

using System;

namespace AlwaysTooLate.CVars
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class ConfigClass : Attribute
    {
        public ConfigClass(string name, bool serializable = false)
        {
        }
    }
}
