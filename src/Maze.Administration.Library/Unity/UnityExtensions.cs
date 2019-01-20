using System;
using System.Linq;
using System.Reflection;
using Maze.Utilities;
using Unity;
using Unity.Lifetime;
using Unity.RegistrationByConvention;

namespace Maze.Administration.Library.Unity
{
    public static class UnityExtensions
    {
        private static bool CheckTypeCompatibility(Type baseType, Type type)
        {
            return !type.IsInterface && !type.IsAbstract && baseType.IsAssignableFrom(type);
        }

        public static void RegisterAssemblyTypes<T>(this IUnityContainer unityContainer, Assembly assembly, Func<Type, LifetimeManager> lifetime)
        {
            var type = typeof(T);
            var types = assembly.ExportedTypes.Where(x => CheckTypeCompatibility(type, x));
            unityContainer.RegisterTypes(types, _ => type.Yield(), WithName.TypeName, lifetime, null, false);
        }

        public static void RegisterAssemblyTypesAsImplementedInterfaces<T>(this IUnityContainer unityContainer, Assembly assembly,
            Func<Type, LifetimeManager> lifetime)
        {
            var type = typeof(T);
            var types = assembly.ExportedTypes.Where(x => CheckTypeCompatibility(type, x));
            unityContainer.RegisterTypes(types, WithMappings.FromAllInterfaces, WithName.TypeName, lifetime);
        }

        public static void AsImplementedInterfaces<TType, TLifetimeManager>(this IUnityContainer unityContainer, string name = null) where TLifetimeManager : LifetimeManager, new()
        {
            var type = typeof(TType);
            var interfaces = type.GetInterfaces();

            unityContainer.RegisterType(type, type, null, new TLifetimeManager());
            foreach (var implementedInterface in interfaces)
                unityContainer.RegisterType(implementedInterface, type, name, new TLifetimeManager());
        }
    }
}