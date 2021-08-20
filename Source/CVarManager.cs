// AlwaysTooLate.CVars (c) 2018-2019 Always Too Late.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AlwaysTooLate.Core;
using UnityEngine;
using UnityEngine.Profiling;

namespace AlwaysTooLate.CVars
{
    public delegate void OnVariableChanged(string name, object value);

    /// <summary>
    ///     Config variable manager class.
    ///     Should be initialized on main (entry) scene.
    /// </summary>
    [DefaultExecutionOrder(-800)]
    public class CVarManager : BehaviourSingleton<CVarManager>
    {
        private static readonly Dictionary<string, ConfigVariable> ConfigVariables =
            new Dictionary<string, ConfigVariable>();

        private static readonly List<KeyValuePair<string, ConfigVariable>> ChangedVariables =
            new List<KeyValuePair<string, ConfigVariable>>(8);

        private static IVariableReplicator _variableReplicator;

        public IReadOnlyDictionary<string, ConfigVariable> AllVariables => ConfigVariables;

        public static bool IsServer { get; set; } = false;

        public static string ConfigDirectory { get; set; } = "Config";
        public static string ConfigFileFormat { get; set; } = string.Concat(ConfigDirectory, "/{0}.json");

        [SerializeField]
        private bool _enableConfigFiles = false;

        protected override void OnAwake()
        {
            Debug.Log("Looking for config variables...");

            ConfigVariables.Clear();
            ChangedVariables.Clear();

            LoadVariables();
        }

        protected void Update()
        {
            Profiler.BeginSample("Update Config Variables");
            UpdateVariables();
            Profiler.EndSample();
        }

        private void UpdateVariables()
        {
            var isDirty = false;

            foreach (var variable in ConfigVariables)
            {
                var variableClass = variable.Value;

                // Update variable
                // When Update returns true, it means that this
                // variable has been changed.
                if (variableClass.Update())
                {
                    isDirty = true;

                    // Add changed variable
                    ChangedVariables.Add(variable);
                }
            }

            if (isDirty)
            {
                // Run replicator and callbacks over selected variables or something
                foreach (var variable in ChangedVariables)
                {
                    var variableClass = variable.Value;

                    // Run replicator if needed
                    if (variableClass.IsReplicated && IsServer)
                        // Run replicator
                        _variableReplicator.OnVariableChanged(variableClass);

                    // Run callbacks
                    OnVariableChanged(variable.Key, variableClass.GetValue());
                }

                // Clear changed variables
                ChangedVariables.Clear();
            }
        }

        private void LoadVariables()
        {
            var timeStart = Time.realtimeSinceStartup;

            // WARNING: Close your eyes, there is a shitload of reflection.

            if (_enableConfigFiles && !Directory.Exists(ConfigDirectory))
                Directory.CreateDirectory(ConfigDirectory);

            // Find all ConfigFiles and ConfigClasses
            var configFiles = ReflectionHelper.GetClassesWithAttribute<ConfigFileAttribute>().ToArray();
            var configClasses = ReflectionHelper.GetClassesWithAttribute<ConfigClassAttribute>().ToArray();

            Debug.Log($"Found {configFiles.Length} config files and {configClasses.Length} config classes.");

            // Load variables from config file classes
            foreach (var configFile in configFiles)
            {
                var attribute = configFile.GetCustomAttribute<ConfigFileAttribute>();
                var configName = attribute.FileName;
                var fileName = string.Format(ConfigFileFormat, configName);

                IConfigFile instance;
                if (_enableConfigFiles && File.Exists(fileName))
                {
                    // Create class instance.
                    instance = Activator.CreateInstance(configFile) as IConfigFile;
                    var instanceProperty = configFile.BaseType?.GetProperty("Current");

                    if (instanceProperty == null)
                    {
                        Debug.LogError(
                            $"Couldn't find instance's Current property. {configName} config file is invalid!");
                        continue;
                    }

                    instanceProperty.SetValue(instance, instance);

                    // Deserialize file
                    instance?.Deserialize(fileName);
                }
                else
                {
                    Debug.Log($"Could not find '{fileName}' config file. Creating new one with default values.");

                    // Create class instance.
                    instance = Activator.CreateInstance(configFile) as IConfigFile;
                    var instanceProperty = configFile.BaseType?.GetProperty("Current");

                    if (instanceProperty == null)
                    {
                        Debug.LogError(
                            $"Couldn't find instance's Current property. {configName} config file is invalid!");
                        continue;
                    }

                    instanceProperty.SetValue(instance, instance);

                    // Serialize file
                    instance?.ResetToDefault();
                    instance?.Serialize(fileName);
                }

                var configGroups = ReflectionHelper.GetClassesWithAttributeSubtype<ConfigGroupAttribute>(configFile);

                if (instance == null)
                {
                    Debug.Log($"Instance of an ConfigFile '{configName}' is null! Cannot load variables.");
                    continue;
                }

                // Find instances of config groups
                foreach (var configGroup in configGroups)
                {
                    var groupName = configGroup.Name;
                    var groupAttribute = configGroup.GetCustomAttribute<ConfigGroupAttribute>();

                    if (!groupName.Contains("Class"))
                    {
                        Debug.LogError($"Invalid ConfigGroup name: {groupName}");
                        continue;
                    }

                    var groupInstanceFieldName = groupName.Replace("Class", "");
                    var groupInstanceField = instance.GetType().GetField(groupInstanceFieldName);

                    if (groupInstanceField == null)
                    {
                        Debug.LogError($"Could not find instance field of name '{groupInstanceFieldName}' " +
                                       $"for config group '{groupName}' " +
                                       $"inside config file '{configName}'.");
                        continue;
                    }

                    var groupInstance = groupInstanceField.GetValue(instance);
                    var configVariables = ReflectionHelper.GetAllConfigVariables(groupInstance);

                    foreach (var variable in configVariables)
                    {
                        var variableAttribute = variable.GetCustomAttribute<ConfigVariableAttribute>();

                        if (variableAttribute == null)
                        {
                            Debug.LogError($"Could not find ConfigVariable attribute for '{variable.Name}' field " +
                                           $"inside config group '{groupName}' " +
                                           $"inside config file '{configName}'.");
                            continue;
                        }

                        // Register variable
                        var variableFullName = $"{groupAttribute.Name}.{variableAttribute.Name}";

                        if (ConfigVariables.ContainsKey(variableFullName))
                        {
                            Debug.LogWarning($"Config variable with key path '{variableFullName}' already exists!");
                            continue;
                        }

                        ConfigVariables.Add(variableFullName, new ConfigVariable
                        {
                            Attribute = variableAttribute,
                            Field = variable,
                            IsStatic = false,
                            GroupType = configGroup,
                            FileType = configFile
                        });
                    }
                }
            }

            foreach (var configClass in configClasses)
            {
                var configClassAttribute = configClass.GetCustomAttribute<ConfigClassAttribute>();
                var configGroups = ReflectionHelper.GetClassesWithAttributeSubtype<ConfigGroupAttribute>(configClass);

                if (configGroups.Any())
                    Debug.LogWarning(
                        "ConfigClass cannot have config groups. They are only meant to be used with ConfigFiles.");

                var configVariables = ReflectionHelper.GetAllConfigVariables(configClass);

                foreach (var variable in configVariables)
                {
                    if (!variable.IsStatic)
                    {
                        Debug.LogError("ConfigClass can only have static fields (not properties!).");
                        continue;
                    }

                    var variableAttribute = variable.GetCustomAttribute<ConfigVariableAttribute>();

                    if (variableAttribute == null)
                    {
                        Debug.LogError($"Could not find ConfigVariable attribute for '{variable.Name}' static field " +
                                       $"inside config class '{configClass.Name}'.");
                        continue;
                    }

                    // Register variable
                    var variableFullName = $"{configClassAttribute.Name}.{variableAttribute.Name}";

                    if (ConfigVariables.ContainsKey(variableFullName))
                    {
                        Debug.LogWarning($"Config variable with key path '{variableFullName}' already exists!");
                        continue;
                    }

                    ConfigVariables.Add(variableFullName, new ConfigVariable
                    {
                        Attribute = variableAttribute,
                        Field = variable,
                        IsStatic = true,
                        GroupType = null,
                        FileType = null
                    });
                }
            }

            var timeEnd = Time.realtimeSinceStartup;

            Debug.Log(
                $"Loaded in total {ConfigVariables.Count} config variables. Loaded in {(timeEnd - timeStart) * 1000.0f:f2}ms.");
        }

        protected virtual void OnVariableChanged(string s, object value)
        {
            VariableChanged?.Invoke(s, value);
        }

        /// <summary>
        ///     Gets variable by name.
        ///     If variable doesn't exists, this function returns null.
        /// </summary>
        /// <param name="name">The variable full name, eg.: cheats.fly</param>
        /// <returns>The variable wrapper class.</returns>
        public static ConfigVariable GetVariable(string name)
        {
            return ConfigVariables.TryGetValue(name, out var variable) ? variable : null;
        }

        /// <summary>
        ///     Sets variable by name.
        /// </summary>
        /// <param name="name">The variable full name, eg.: cheats.fly</param>
        /// <param name="value">The new variable value. Make sure that it is the proper type.</param>
        public static void SetVariable(string name, object value)
        {
            ConfigVariables[name].SetValue(value);
        }

        /// <summary>
        ///     Gets all config files.
        /// </summary>
        /// <returns>The config file.</returns>
        public static IEnumerable<IConfigFile> GetConfigFiles()
        {
            var configFileInstances = new List<IConfigFile>();
            var configFiles = ReflectionHelper.GetClassesWithAttribute<ConfigFileAttribute>().ToArray();
            foreach (var configFile in configFiles)
            {
                var instanceProperty = configFile.BaseType?.GetProperty("Current");
                var instance = (IConfigFile) instanceProperty?.GetValue(null);
                configFileInstances.Add(instance);
            }

            return configFileInstances;
        }

        /// <summary>
        ///     Saves all config files.
        /// </summary>
        public static void SaveAll()
        {
            if (!Instance._enableConfigFiles) return;
        
            // Save all config files
            foreach (var configFile in GetConfigFiles()) configFile.Save();
        }

        /// <summary>
        ///     Sets config variable replicator.
        /// </summary>
        /// <param name="replicator">The replicator instance.</param>
        /// <remarks>
        ///     Can be used only on server.
        /// </remarks>
        public static void SetVariableReplicator(IVariableReplicator replicator)
        {
            Debug.Assert(IsServer,
                "Only server can get variable replicator. As server only has the power, to replicate the wor... variables.");

            _variableReplicator = replicator;
        }

        public event OnVariableChanged VariableChanged;
    }
}
