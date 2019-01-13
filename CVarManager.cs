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
            // TODO: Find all ConfigClasses and ConfigVariables.
        }
    }
}
