using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace Services.Common.DI
{
    /// <summary>
    /// TODO : Hung.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DependencyAttribute : Attribute
    {
        protected DependencyAttribute(ServiceLifetime dependencyType)
        {
            DependencyType = dependencyType;
        }

        public ServiceLifetime DependencyType { get; }
        public Type ServiceType { get; set; }

        /// <summary>
        /// Registering an type as an interface and as self.
        /// </summary>
        public bool AsSelf { get; set; }

        public ServiceDescriptor BuildServiceDescriptor(TypeInfo type)
        {
            var serviceType = ServiceType ?? type.AsType();

            return new ServiceDescriptor(serviceType, type.AsType(), DependencyType);
        }
    }
}