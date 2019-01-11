using System;
using Microsoft.Extensions.DependencyInjection;
using Unity;
using Unity.Lifetime;

namespace Maze.Administration.Services
{
    public class UnityServiceProvider : IServiceProvider, IServiceScopeFactory, IServiceScope, IDisposable
    {
        private IUnityContainer _container;

        public UnityServiceProvider(IUnityContainer container)
        {
            _container = container;
            _container.RegisterInstance<IServiceScope>(this, new ExternallyControlledLifetimeManager());
            _container.RegisterInstance<IServiceProvider>(this, new ExternallyControlledLifetimeManager());
            _container.RegisterInstance<IServiceScopeFactory>(this, new ExternallyControlledLifetimeManager());
        }

        #region IServiceProvider

        public object GetService(Type serviceType)
        {
            try
            {
                return _container.Resolve(serviceType);
            }
            catch
            {
                /* Ignore */
            }

            return null;
        }

        #endregion


        #region IServiceScope

        IServiceProvider IServiceScope.ServiceProvider => this;

        #endregion


        #region IServiceScopeFactory

        public IServiceScope CreateScope() => new UnityServiceProvider(_container.CreateChildContainer());

        #endregion


        #region Public Members

        public static explicit operator UnityContainer(UnityServiceProvider c) => (UnityContainer) c._container;

        #endregion


        #region Disposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool _)
        {
            IDisposable disposable = _container;
            _container = null;
            disposable?.Dispose();
        }

        #endregion
    }
}