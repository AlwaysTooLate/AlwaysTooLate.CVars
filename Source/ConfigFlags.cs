// AlwaysTooLate.CVars (c) 2018-2022 Always Too Late.

using System;

namespace AlwaysTooLate.CVars
{
    [Flags]
    public enum ConfigFlags
    {
        None = 0,

        /// <summary>
        ///     When set, variable will be using IVariableReplicator bound the the CVarManager using
        ///     CVarManager.BindReplicator(...).
        ///     When variable is being changed, server will send this variable through the replicator to
        ///     all clients.
        /// </summary>
        Replicate = 1 << 0,

        Default = None
    }
}