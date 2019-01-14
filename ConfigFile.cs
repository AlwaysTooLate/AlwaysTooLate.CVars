// AlwaysTooLate.CVars (c) 2018-2019 Always Too Late.

using System;
using System.IO;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;

namespace AlwaysTooLate.CVars
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ConfigFileAttribute : Attribute
    {
        public ConfigFileAttribute(string fileName)
        {
            FileName = fileName;
        }

        public string FileName { get; }
    }

    public interface IConfigFile
    {
        void Save();
        void Load();

        void Serialize(string file);
        void Deserialize(string file);
        void ResetToDefault();
    }

    public class ConfigFile<TClass> : IConfigFile where TClass : new()
    {
        public string GetSaveFile()
        {
            var attribute = typeof(TClass).GetCustomAttribute<ConfigFileAttribute>();
            var configName = attribute.FileName;
            return string.Format(CVarManager.ConfigFileFormat, configName);
        }

        public void Save()
        {
            var fileName = GetSaveFile();
            Serialize(fileName);
        }

        public void Load()
        {
            var fileName = GetSaveFile();
            Deserialize(fileName);

        }
        
        public void Serialize(string file)
        {
            var json = JsonUtility.ToJson(this, true);
            File.WriteAllText(file, json);
        }

        public void Deserialize(string file)
        {
            var json = File.ReadAllText(file);
            Current = JsonUtility.FromJson<TClass>(json);
        }

        public void ResetToDefault()
        {
            var baseType = typeof(TClass);
            var configGroups = ReflectionHelper.GetClassesWithAttributeSubtype<ConfigGroupAttribute>(baseType);

            foreach (var configGroup in configGroups)
            {
                var groupName = configGroup.Name;

                if (!groupName.Contains("Class"))
                {
                    Debug.LogError($"Invalid ConfigGroup name: {groupName}");
                    continue;
                }

                var groupInstanceFieldName = groupName.Replace("Class", "");
                var groupInstanceField = baseType.GetField(groupInstanceFieldName);

                if (groupInstanceField == null)
                {
                    Debug.LogError($"Could not find instance field of name '{groupInstanceFieldName}' " +
                                   $"for config group '{groupName}' " +
                                   $"inside config file '{nameof(TClass)}'.");
                    continue;
                }

                var groupInstance = (ConfigGroup)groupInstanceField.GetValue(this);

                if (groupInstance == null)
                {
                    Debug.LogError($"Could not find instance of config group '{groupInstanceFieldName}'.");
                    continue;
                }

                // Call reset
                groupInstance.ResetToDefault();
            }
        }

        [UsedImplicitly]
        public static TClass Current { get; private set; }
    }
}
