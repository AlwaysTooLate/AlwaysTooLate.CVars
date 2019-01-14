// AlwaysTooLate.Core (c) 2018-2019 Always Too Late. All rights reserved.

using AlwaysTooLate.Core;

namespace AlwaysTooLate.CVars
{
    /// <summary>
    /// Config variable manager class.
    /// Should be initialized on main (entry) scene.
    /// </summary>
    public class CVarManager : BehaviourSingleton<CVarManager>
    {
        protected override void OnAwake()
        {
            // TODO: Find all ConfigFiles (instances)
            // TODO: Find all ConfigClasses (static only fields)
        }
        /*
        public List<Type> GetConfigClasses()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes());

            var configClasses = new List<Type>();
            foreach (var type in types)
            {
                var classAttributes = type.GetCustomAttributes(false).Where(attribute => attribute is ConfigClass).ToArray();

                if (classAttributes.Length > 0)
                {
                    configClasses.Add(type);
                }
            }

            return configClasses;
        }*/
    }
}
