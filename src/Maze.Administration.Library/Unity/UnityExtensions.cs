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
        public static void RegisterAssemblyTypes<T>(this IUnityContainer unityContainer, Assembly assembly, Func<Type, LifetimeManager> lifetime)
        {
            var type = typeof(T);
            unityContainer.RegisterTypes(assembly.ExportedTypes.Where(x => x.IsAssignableFrom(type)), _ => type.Yield(), null, lifetime);
        }

        public static void RegisterAssemblyTypesAsImplementedInterfaces<T>(this IUnityContainer unityContainer, Assembly assembly, Func<Type, LifetimeManager> lifetime)
        {
            var type = typeof(T);
            unityContainer.RegisterTypes(assembly.ExportedTypes.Where(x => x.IsAssignableFrom(type)), _ => type.GetInterfaces(), null, lifetime);
        }

        public static void AsImplementedInterfaces<TType, TLifetimeManager>(this IUnityContainer unityContainer) where TLifetimeManager : LifetimeManager, new()
        {
            var type = typeof(TType);
            var interfaces = type.GetInterfaces();

            unityContainer.RegisterType(type, type, null, new TLifetimeManager());
            foreach (var implementedInterface in interfaces)
                unityContainer.RegisterType(implementedInterface, type, null, new TLifetimeManager());
        }
    }
}