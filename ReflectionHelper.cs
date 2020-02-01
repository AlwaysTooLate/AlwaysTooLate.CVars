// AlwaysTooLate.CVars (c) 2018-2019 Always Too Late.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AlwaysTooLate.CVars
{
    internal static class ReflectionHelper
    {
        internal static IEnumerable<FieldInfo> GetAllConfigVariables(object instance)
        {
            var type = instance.GetType();
            return GetAllConfigVariables(type);
        }

        internal static IEnumerable<FieldInfo> GetAllConfigVariables(Type type)
        {
            var fields = new List<FieldInfo>();

            fields.AddRange(type.GetFields());

            return fields;
        }

        internal static IEnumerable<Type> GetClassesWithAttributeSubtype<TAttribute>(Type baseType)
            where TAttribute : class
        {
            var configClasses = new List<Type>();
            var types = baseType.GetNestedTypes();
            foreach (var type in types)
                if (type.GetCustomAttributes(typeof(TAttribute)).Any())
                    configClasses.Add(type);

            return configClasses;
        }

        internal static IEnumerable<Type> GetClassesWithAttribute<TAttribute>() where TAttribute : class
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes());

            var configClasses = new List<Type>();
            foreach (var type in types)
                if (type.GetCustomAttributes(typeof(TAttribute)).Any())
                    configClasses.Add(type);

            return configClasses;
        }
    }
}