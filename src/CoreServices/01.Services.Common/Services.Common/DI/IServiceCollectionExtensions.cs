using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Module = Services.Common.Modules.Module;

namespace Services.Common.DI
{
    public static class IServiceCollectionExtensions
    {
        #region Scoped

        public static IServiceCollection AddScopedIfAny<TService, TImplementation>(this IServiceCollection services, [NotNull] Func<ServiceDescriptor, bool> predicate) where TService : class where TImplementation : class, TService
        {
            if (services.Any(predicate))
            {
                services.AddScoped<TService, TImplementation>();
            }

            return services;
        }

        public static IServiceCollection AddScopedIfAny<TService>(this IServiceCollection services, [NotNull] Func<ServiceDescriptor, bool> predicate) where TService : class
        {
            if (services.Any(predicate))
            {
                services.AddScoped<TService>();
            }

            return services;
        }

        public static IServiceCollection AddScopedIfAll<TService, TImplementation>(this IServiceCollection services, [NotNull] Func<ServiceDescriptor, bool> predicate) where TService : class where TImplementation : class, TService
        {
            if (services.All(predicate))
            {
                services.AddScoped<TService, TImplementation>();
            }

            return services;
        }

        public static IServiceCollection AddScopedIfAll<TService>(this IServiceCollection services, [NotNull] Func<ServiceDescriptor, bool> predicate) where TService : class
        {
            if (services.All(predicate))
            {
                services.AddScoped<TService>();
            }

            return services;
        }

        public static IServiceCollection AddScopedIfNotExist<TService, TImplementation>(this IServiceCollection services) where TService : class where TImplementation : class, TService
        {
            services.AddScopedIfAll<TService, TImplementation>(x => x.ServiceType != typeof(TService));
            return services;
        }

        public static IServiceCollection AddScopedIfNotExist<TService>(this IServiceCollection services) where TService : class
        {
            services.AddScopedIfAll<TService>(x => x.ServiceType != typeof(TService));
            return services;
        }

        #endregion Scoped

        #region Transient

        public static IServiceCollection AddTransientIfAny<TService, TImplementation>(this IServiceCollection services, [NotNull] Func<ServiceDescriptor, bool> predicate) where TService : class where TImplementation : class, TService
        {
            if (services.Any(predicate))
            {
                services.AddTransient<TService, TImplementation>();
            }

            return services;
        }

        public static IServiceCollection AddTransientIfAny<TService>(this IServiceCollection services, [NotNull] Func<ServiceDescriptor, bool> predicate) where TService : class
        {
            if (services.Any(predicate))
            {
                services.AddTransient<TService>();
            }

            return services;
        }

        public static IServiceCollection AddTransientIfNotExist<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            services.AddTransientIfAll<TService, TImplementation>(x => x.ServiceType != typeof(TService));
            return services;
        }

        public static IServiceCollection AddTransientIfNotExist<TService>(this IServiceCollection services)
            where TService : class
        {
            services.AddTransientIfAll<TService>(x => x.ServiceType != typeof(TService));
            return services;
        }

        public static IServiceCollection AddTransientIfAll<TService, TImplementation>(this IServiceCollection services, [NotNull] Func<ServiceDescriptor, bool> predicate)
            where TService : class where TImplementation : class, TService
        {
            if (services.All(predicate))
            {
                services.AddTransient<TService, TImplementation>();
            }

            return services;
        }

        public static IServiceCollection AddTransientIfAll<TService>(this IServiceCollection services, [NotNull] Func<ServiceDescriptor, bool> predicate) where TService : class
        {
            if (services.All(predicate))
            {
                services.AddTransient<TService>();
            }

            return services;
        }

        #endregion Transient

        #region Singleton

        public static IServiceCollection AddSingletonIfAny<TService, TImplementation>(this IServiceCollection services, [NotNull] Func<ServiceDescriptor, bool> predicate)
            where TService : class where TImplementation : class, TService
        {
            if (services.Any(predicate))
            {
                services.AddSingleton<TService, TImplementation>();
            }

            return services;
        }

        public static IServiceCollection AddSingletonIfAny<TService>(this IServiceCollection services, [NotNull] Func<ServiceDescriptor, bool> predicate) where TService : class
        {
            if (services.Any(predicate))
            {
                services.AddSingleton<TService>();
            }

            return services;
        }

        public static IServiceCollection AddSingletonIfAll<TService, TImplementation>(this IServiceCollection services, [NotNull] Func<ServiceDescriptor, bool> predicate)
            where TService : class where TImplementation : class, TService
        {
            if (services.All(predicate))
            {
                services.AddSingleton<TService, TImplementation>();
            }

            return services;
        }

        public static IServiceCollection AddSingletonIfAll<TService>(this IServiceCollection services, [NotNull] Func<ServiceDescriptor, bool> predicate) where TService : class
        {
            if (services.All(predicate))
            {
                services.AddSingleton<TService>();
            }

            return services;
        }

        public static IServiceCollection AddSingletonIfNotExist<TService, TImplementation>(this IServiceCollection services) where TService : class where TImplementation : class, TService
        {
            services.AddSingletonIfAll<TService, TImplementation>(x => x.ServiceType != typeof(TService));
            return services;
        }

        public static IServiceCollection AddSingletonIfNotExist<TService>(this IServiceCollection services)
            where TService : class
        {
            services.AddSingletonIfAll<TService>(x => x.ServiceType != typeof(TService));
            return services;
        }

        #endregion Singleton

        #region Scan

        public static bool IsRegistered<TService>(this IServiceCollection services)
        {
            bool isExist = services.All(x => x.ServiceType == typeof(TService));

            return isExist;
        }

        public static IEnumerable<Assembly> AddDI<TStartupModule>(this IServiceCollection services) where TStartupModule : Module
        {
            var modules = Module.FindDependedModuleTypesRecursivelyIncludingGivenModule(typeof(TStartupModule));

            var assembliesByModules = modules.Select(x => x.Assembly);

            Scan(services, assembliesByModules);

            return assembliesByModules;
        }

        /// <summary>
        /// Scan supplied assemblies and bind them automatically to service collection based on attribute and convention
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        /// <param name="options"></param>
        private static void Scan(this IServiceCollection services, IEnumerable<Assembly> assemblies, IocScannerOptions options = null)
        {
            options ??= new IocScannerOptions();

            var skipAutoBindAttributeFullName = typeof(SkipAutoBindAttribute).FullName;

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                    var typeInfo = type.GetTypeInfo();

                    var interfaces = typeInfo.GetInterfaces();

                    var isAbstract = typeInfo.IsAbstract || typeInfo.IsInterface;

                    if (isAbstract) continue;

                    var attributes = typeInfo.GetCustomAttributes().ToList();

                    if (attributes.Any(a => a.GetType().FullName == skipAutoBindAttributeFullName)) continue;

                    if (attributes.Any())
                    {
                        foreach (var attribute in attributes)
                        {
                            var isDependencyAttribute = attribute is DependencyAttribute;

                            if (!isDependencyAttribute)
                            {
                                continue;
                            }

                            var dependencyAttribute = (DependencyAttribute)attribute;

                            var serviceDescriptor = dependencyAttribute.BuildServiceDescriptor(typeInfo);

                            var implementationTypeRegistered = services
                                .FirstOrDefault(x => x.ServiceType.FullName == serviceDescriptor.ServiceType.FullName && x.ImplementationType != serviceDescriptor.ImplementationType)?
                                .ImplementationType;

                            // Check is service already register from difference implementation => throw exception
                            if (implementationTypeRegistered != null)
                            {
                                throw new NotSupportedException($"Conflict implementation, ${serviceDescriptor.ImplementationType} try to register for {serviceDescriptor.ServiceType.FullName} but it already register by {implementationTypeRegistered.FullName} before.");
                            }

                            // Check is service already register from same implementation => remove existing,
                            // replace by new one to make use last define life time cycle

                            var isExistSameImplementationRegistered = services.Any(x => x.ServiceType.FullName == serviceDescriptor.ServiceType.FullName && x.ImplementationType == serviceDescriptor.ImplementationType);

                            if (isExistSameImplementationRegistered)
                            {
                                services = services.Replace(serviceDescriptor);

                                if (dependencyAttribute.AsSelf)
                                {
                                    var serviceDescriptorAsSelf = new ServiceDescriptor(typeInfo.AsType(), p => p.GetService(serviceDescriptor.ServiceType), serviceDescriptor.Lifetime);

                                    services.Replace(serviceDescriptorAsSelf);
                                }
                            }
                            else
                            {
                                services.Add(serviceDescriptor);

                                if (dependencyAttribute.AsSelf)
                                {
                                    var serviceDescriptorAsSelf = new ServiceDescriptor(typeInfo.AsType(), p => p.GetService(serviceDescriptor.ServiceType), serviceDescriptor.Lifetime);

                                    services.Add(serviceDescriptorAsSelf);
                                }
                            }
                        }
                        continue;
                    }

                    if (!attributes.Any() && interfaces.Any())
                    {
                        CompareInfo myComp = CultureInfo.CurrentCulture.CompareInfo;

                        var inValidInterfaces = interfaces.Where(x => options.CommonPostfixes.Any(t => myComp.IsSuffix(x.Name, t))).ToList();

                        foreach (var interfaceImplemented in inValidInterfaces)
                        {
                            services.TryAdd(ServiceDescriptor.Transient(interfaceImplemented, type));
                        }
                    }
                }
            }
        }

        #endregion Scan
    }
}