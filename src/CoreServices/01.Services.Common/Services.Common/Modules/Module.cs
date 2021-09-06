using Services.Common.Collections.Extensions;
using Services.Common.ExceptionsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Services.Common.Modules
{
    public abstract class Module
    {
        /// <summary>
        /// Checks if given type is an FW module class.
        /// </summary>
        /// <param name="type">Type to check</param>
        public static bool IsModule(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            return
                typeInfo.IsClass &&
                !typeInfo.IsAbstract &&
                !typeInfo.IsGenericType &&
                typeof(Module).IsAssignableFrom(type);
        }

        public static List<Type> FindDependedModuleTypesRecursivelyIncludingGivenModule(Type moduleType)
        {
            var list = new List<Type>();

            AddModuleAndDependenciesRecursively(list, moduleType);

            list.AddIfNotContains(typeof(KernelModule));

            return list;
        }

        /// <summary>
        /// Finds direct depended modules of a module (excluding given module).
        /// </summary>
        public static List<Type> FindDependedModuleTypes(Type moduleType)
        {
            if (!IsModule(moduleType))
            {
                throw new InitializationException("This type is not an FW module: " + moduleType.AssemblyQualifiedName);
            }

            var list = new List<Type>();

            if (moduleType.GetTypeInfo().IsDefined(typeof(DependsOnAttribute), true))
            {
                var dependsOnAttributes = moduleType.GetTypeInfo().GetCustomAttributes(typeof(DependsOnAttribute), true).Cast<DependsOnAttribute>();

                foreach (var dependsOnAttribute in dependsOnAttributes)
                {
                    foreach (var dependedModuleType in dependsOnAttribute.DependedModuleTypes)
                    {
                        list.Add(dependedModuleType);
                    }
                }
            }

            return list;
        }

        private static void AddModuleAndDependenciesRecursively(List<Type> modules, Type module)
        {
            if (!IsModule(module))
            {
                throw new InitializationException("This type is not an FW module: " + module.AssemblyQualifiedName);
            }

            if (modules.Contains(module))
            {
                return;
            }

            modules.Add(module);

            var dependedModules = FindDependedModuleTypes(module);

            foreach (var dependedModule in dependedModules)
            {
                AddModuleAndDependenciesRecursively(modules, dependedModule);
            }
        }
    }
}