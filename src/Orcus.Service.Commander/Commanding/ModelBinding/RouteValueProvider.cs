using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Orcus.Modules.Api.ModelBinding;
using Orcus.Modules.Api.Parameters;
using Orcus.Service.Commander.Commanding.Internal;

namespace Orcus.Service.Commander.Commanding.ModelBinding
{
    /// <summary>
    /// An <see cref="IValueProvider"/> adapter for data stored in an <see cref="RouteValueDictionary"/>.
    /// </summary>
    public class RouteValueProvider : IValueProvider
    {
        private readonly IReadOnlyDictionary<string, object> _values;
        private PrefixContainer _prefixContainer;

        /// <summary>
        /// Creates a new <see cref="RouteValueProvider"/>.
        /// </summary>
        /// <param name="bindingSource">The <see cref="BindingSource"/> of the data.</param>
        /// <param name="values">The values.</param>
        /// <remarks>Sets <see cref="Culture"/> to <see cref="CultureInfo.InvariantCulture" />.</remarks>
        public RouteValueProvider(
            BindingSource bindingSource,
            IReadOnlyDictionary<string, object> values)
            : this(bindingSource, values, CultureInfo.InvariantCulture)
        {
        }

        /// <summary>
        /// Creates a new <see cref="RouteValueProvider"/>.
        /// </summary>
        /// <param name="bindingSource">The <see cref="BindingSource"/> of the data.</param>
        /// <param name="values">The values.</param>
        /// <param name="culture">The culture for route value.</param>
        public RouteValueProvider(BindingSource bindingSource, IReadOnlyDictionary<string, object> values, CultureInfo culture)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (culture == null)
            {
                throw new ArgumentNullException(nameof(culture));
            }

            _values = values;
            Culture = culture;
        }
        
        protected PrefixContainer PrefixContainer
        {
            get
            {
                if (_prefixContainer == null)
                {
                    _prefixContainer = new PrefixContainer(_values.Keys.ToList());
                }

                return _prefixContainer;
            }
        }

        protected CultureInfo Culture { get; }

        /// <inheritdoc />
        public bool ContainsPrefix(string key)
        {
            return PrefixContainer.ContainsPrefix(key);
        }

        /// <inheritdoc />
        public ValueProviderResult GetValue(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            object value;
            if (_values.TryGetValue(key, out value))
            {
                var stringValue = value as string ?? value?.ToString() ?? string.Empty;
                return new ValueProviderResult(stringValue, Culture);
            }
            else
            {
                return ValueProviderResult.None;
            }
        }
    }
}