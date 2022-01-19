// AlwaysTooLate.CVars (c) 2018-2022 Always Too Late.

using System;
using System.Reflection;
using UnityEngine;

namespace AlwaysTooLate.CVars
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class ConfigVariableAttribute : Attribute
    {
        public ConfigVariableAttribute(
            string name,
            string description = "",
            object defaultValue = null,
            ConfigAccess access = ConfigAccess.Any,
            ConfigFlags flags = ConfigFlags.Default)
        {
            Name = name;
            Description = description;
            DefaultValue = defaultValue;
            Access = access;
            Flags = flags;
        }

        public string Name { get; }
        public string Description { get; }
        public object DefaultValue { get; }
        public ConfigAccess Access { get; }
        public ConfigFlags Flags { get; }
    }

    public class ConfigVariable
    {
        private object _baseInstance;
        private object _groupInstance;
        private object _lastValue;

        public ConfigVariableAttribute Attribute { get; set; }
        public Type FileType { get; set; }
        public Type GroupType { get; set; }
        public FieldInfo Field { get; set; }
        public bool IsStatic { get; set; }
        public bool IsReplicated => (Attribute.Flags & ConfigFlags.Replicate) != 0;

        public bool Update()
        {
            if (_lastValue == GetValue())
                return false;

            // TODO: Reset to previous value, when there is different access to the variable (CVarManager.IsServer)

            _lastValue = GetValue();
            return true;
        }

        public void SetValue(object value)
        {
            if (value.GetType() != Field.FieldType)
            {
                Debug.LogWarning("Could not set variable. Invalid type.");
                return;
            }

            // TODO: Do not change value, when there is different access to the variable (CVarManager.IsServer)

            // Works both for static and non-static variables
            Field.SetValue(GetGroupInstance(), value);
        }

        public object GetValue()
        {
            // Works both for static and non-static variables
            return Field.GetValue(GetGroupInstance());
        }

        public object GetGroupInstance()
        {
            if (IsStatic)
                return null;

            if (_groupInstance != null) // Try get instance from cache
                return _groupInstance;

            var instance = GetBaseInstance();
            var groupName = GroupType.Name;
            var groupInstanceFieldName = groupName.Replace("Class", "");
            var groupInstanceField = instance?.GetType().GetField(groupInstanceFieldName);

            _groupInstance = groupInstanceField?.GetValue(instance); // Cache the instance object
            return _groupInstance;
        }

        public object GetBaseInstance()
        {
            if (IsStatic)
                return null;

            if (_baseInstance != null) // Try get instance from cache
                return _baseInstance;

            var instanceProperty = FileType.BaseType?.GetProperty("Current");

            _baseInstance = instanceProperty?.GetValue(null); // Cache the instance object
            return _baseInstance;
        }
    }
}