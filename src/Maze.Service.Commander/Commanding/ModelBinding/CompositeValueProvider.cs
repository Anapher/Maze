using System.Collections.Generic;
using System.Collections.ObjectModel;
using Maze.Modules.Api.ModelBinding;

namespace Maze.Service.Commander.Commanding.ModelBinding
{
    /// <summary>
    /// Represents a <see cref="IValueProvider"/> whose values come from a collection of <see cref="IValueProvider"/>s.
    /// </summary>
    public class CompositeValueProvider : Collection<IValueProvider>, IValueProvider
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CompositeValueProvider"/>.
        /// </summary>
        public CompositeValueProvider()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="CompositeValueProvider"/>.
        /// </summary>
        /// <param name="valueProviders">The sequence of <see cref="IValueProvider"/> to add to this instance of
        /// <see cref="CompositeValueProvider"/>.</param>
        public CompositeValueProvider(IList<IValueProvider> valueProviders)
            : base(valueProviders)
        {
        }

        /// <inheritdoc />
        public virtual bool ContainsPrefix(string prefix)
        {
            for (var i = 0; i < Count; i++)
            {
                if (this[i].ContainsPrefix(prefix))
                {
                    return true;
                }
            }
            return false;
        }

        /// <inheritdoc />
        public virtual ValueProviderResult GetValue(string key)
        {
            // Performance-sensitive
            // Caching the count is faster for IList<T>
            var itemCount = Items.Count;
            for (var i = 0; i < itemCount; i++)
            {
                var valueProvider = Items[i];
                var result = valueProvider.GetValue(key);
                if (result != ValueProviderResult.None)
                {
                    return result;
                }
            }

            return ValueProviderResult.None;
        }
    }
}