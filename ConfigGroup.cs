﻿// AlwaysTooLate.Core (c) 2018-2019 Always Too Late. All rights reserved.

using System;

namespace AlwaysTooLate.CVars
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class ConfigGroup : Attribute
    {
        public ConfigGroup(string name)
        {
        }
    }
}