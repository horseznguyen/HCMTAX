using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Services.Common.Modules
{
    public class ModuleInfo
    {
        /// <summary>
        /// The assembly which contains the module definition.
        /// </summary>
        public Assembly Assembly { get; }

        /// <summary>
        /// Type of the module.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Instance of the module.
        /// </summary>
        public Module Instance { get; }

        /// <summary>
        /// All dependent modules of this module.
        /// </summary>
        public List<ModuleInfo> Dependencies { get; }

        /// <summary>
        /// Creates a new ModuleInfo object.
        /// </summary>
        public ModuleInfo([NotNull] Type type, [NotNull] Module instance)
        {
            Type = type;
            Instance = instance;
            Assembly = Type.GetTypeInfo().Assembly;
            Dependencies = new List<ModuleInfo>();
        }

        public override string ToString()
        {
            return Type.AssemblyQualifiedName ?? Type.FullName;
        }
    }
}