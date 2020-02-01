// AlwaysTooLate.CVars (c) 2018-2019 Always Too Late.

using System;
using System.Reflection;
using UnityEngine;

namespace AlwaysTooLate.CVars
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class ConfigGroupAttribute : Attribute
    {
        public ConfigGroupAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    [Serializable]
    public class ConfigGroup
    {
        public void ResetToDefault()
        {
            var configVariables = ReflectionHelper.GetAllConfigVariables(this);

            foreach (var configVariable in configVariables)
            {
                var variableAttribute = configVariable.GetCustomAttribute<ConfigVariableAttribute>();

                if (variableAttribute == null)
                {
                    Debug.LogError(
                        $"Could not find ConfigVariable attribute for '{configVariable.Name}' inside '{this}'.");
                    continue;
                }

                // Set default values
                configVariable.SetValue(this, variableAttribute.DefaultValue);
            }
        }
    }
}