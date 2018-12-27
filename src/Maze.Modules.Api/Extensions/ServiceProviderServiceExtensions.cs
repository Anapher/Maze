using System;

namespace Orcus.Modules.Api.Extensions
{
    public static class ServiceProviderServiceExtensions
    {
        /// <summary>
        ///     Get service of type <typeparamref name="T" /> from the <see cref="T:System.IServiceProvider" />.
        /// </summary>
        /// <typeparam name="T">The type of service object to get.</typeparam>
        /// <param name="provider">The <see cref="T:System.IServiceProvider" /> to retrieve the service object from.</param>
        /// <returns>A service object of type <typeparamref name="T" />.</returns>
        /// <exception cref="T:System.InvalidOperationException">There is no service of type <typeparamref name="T" />.</exception>
        public static T GetRequiredService<T>(this IServiceProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            return (T) provider.GetService(typeof(T));
        }
    }
}