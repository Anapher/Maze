using System;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Orcus.Client.Library.Extensions
{
    /// <summary>
    ///     Extension methods for adding configuration related options services to the DI container.
    /// </summary>
    public static class OptionsConfigurationAutofacExtensions
    {
        /// <summary>
        ///     Registers a configuration instance which TOptions will bind against.
        /// </summary>
        /// <typeparam name="TOptions">The type of options being configured.</typeparam>
        /// <param name="services">
        ///     The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> to add the
        ///     services to.
        /// </param>
        /// <param name="config">The configuration being bound.</param>
        /// <returns>
        ///     The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> so that additional calls can
        ///     be chained.
        /// </returns>
        public static ContainerBuilder Configure<TOptions>(this ContainerBuilder services, IConfiguration config)
            where TOptions : class =>
            services.Configure<TOptions>(Microsoft.Extensions.Options.Options.DefaultName, config);

        /// <summary>
        ///     Registers a configuration instance which TOptions will bind against.
        /// </summary>
        /// <typeparam name="TOptions">The type of options being configured.</typeparam>
        /// <param name="services">
        ///     The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> to add the
        ///     services to.
        /// </param>
        /// <param name="name">The name of the options instance.</param>
        /// <param name="config">The configuration being bound.</param>
        /// <returns>
        ///     The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> so that additional calls can
        ///     be chained.
        /// </returns>
        public static ContainerBuilder Configure<TOptions>(this ContainerBuilder services, string name,
            IConfiguration config) where TOptions : class
        {
            return services.Configure<TOptions>(name, config, _ => { });
        }

        /// <summary>
        ///     Registers a configuration instance which TOptions will bind against.
        /// </summary>
        /// <typeparam name="TOptions">The type of options being configured.</typeparam>
        /// <param name="services">
        ///     The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> to add the
        ///     services to.
        /// </param>
        /// <param name="config">The configuration being bound.</param>
        /// <param name="configureBinder">Used to configure the <see cref="T:Microsoft.Extensions.Configuration.BinderOptions" />.</param>
        /// <returns>
        ///     The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> so that additional calls can
        ///     be chained.
        /// </returns>
        public static ContainerBuilder Configure<TOptions>(this ContainerBuilder services, IConfiguration config,
            Action<BinderOptions> configureBinder) where TOptions : class =>
            services.Configure<TOptions>(Microsoft.Extensions.Options.Options.DefaultName, config, configureBinder);

        /// <summary>
        ///     Registers a configuration instance which TOptions will bind against.
        /// </summary>
        /// <typeparam name="TOptions">The type of options being configured.</typeparam>
        /// <param name="services">
        ///     The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> to add the
        ///     services to.
        /// </param>
        /// <param name="name">The name of the options instance.</param>
        /// <param name="config">The configuration being bound.</param>
        /// <param name="configureBinder">Used to configure the <see cref="T:Microsoft.Extensions.Configuration.BinderOptions" />.</param>
        /// <returns>
        ///     The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> so that additional calls can
        ///     be chained.
        /// </returns>
        public static ContainerBuilder Configure<TOptions>(this ContainerBuilder services, string name,
            IConfiguration config, Action<BinderOptions> configureBinder) where TOptions : class
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            services.RegisterInstance<IOptionsChangeTokenSource<TOptions>>(
                new ConfigurationChangeTokenSource<TOptions>(name, config));
            services.RegisterInstance<IConfigureOptions<TOptions>>(
                new NamedConfigureFromConfigurationOptions<TOptions>(name, config, configureBinder));

            return services;
        }

        /// <returns>
        ///     The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> so that additional calls can
        ///     be chained.
        /// </returns>
        public static ContainerBuilder AddOptions(this ContainerBuilder services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.RegisterGeneric(typeof(OptionsManager<>)).As(typeof(IOptions<>)).SingleInstance();
            services.RegisterGeneric(typeof(OptionsManager<>)).As(typeof(IOptionsSnapshot<>))
                .InstancePerLifetimeScope();
            services.RegisterGeneric(typeof(OptionsMonitor<>)).As(typeof(IOptionsMonitor<>)).InstancePerLifetimeScope();
            services.RegisterGeneric(typeof(OptionsFactory<>)).As(typeof(IOptionsFactory<>)).InstancePerLifetimeScope();
            services.RegisterGeneric(typeof(OptionsCache<>)).As(typeof(IOptionsMonitorCache<>))
                .InstancePerLifetimeScope();

            return services;
        }
    }
}