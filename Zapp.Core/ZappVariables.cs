namespace Zapp.Core
{
    /// <summary>
    /// Represents a collaboration of variables across libraries.
    /// </summary>
    public static class ZappVariables
    {
        /// <summary>
        /// Represents the name of the fusion meta entry.
        /// </summary>
        public const string FusionMetaEntyName = "fusion-meta.json";

        /// <summary>
        /// Represents the evironment key for the fusions id.
        /// </summary>
        public const string FusionIdEnvKey = "fusion.id";

        /// <summary>
        /// Represents the environment key of parent process' id.
        /// </summary>
        public const string ParentProcessIdEnvKey = "parent.id";

        /// <summary>
        /// Represents the environment key of parent rest port;
        /// </summary>
        public const string ParentPortEnvKey = "parent.port";

        /// <summary>
        /// Represents the fusion info key of the startup assembly name.
        /// </summary>

        public const string StartupAssemblyNameFusionInfoKey = "startup.assembly.name";

        /// <summary>
        /// Represents the fusion info key of the teardown assembly name.
        /// </summary>
        public const string TeardownAssemblyNameFusionInfoKey = "teardown.assembly.name";

        /// <summary>
        /// Represents the fusion info key of the teardown type name.
        /// </summary>
        public const string TeardownTypeNameFusionInfoKey = "teardown.type.name";

        /// <summary>
        /// Represents the fusion info key of the teardown method name.
        /// </summary>
        public const string TeardownMethodNameFusionInfoKey = "teardown.method.name";
    }
}
