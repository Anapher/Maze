using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Orcus.Administration.Core.Extensions;

namespace Orcus.Administration.Core.Clients.Helpers
{
    public static class RequestBuilderExtensions
    {
        public static IRequestBuilder WithBody(this IRequestBuilder requestBuilder, object body)
        {
            requestBuilder.Content = new JsonContent(body);
            return requestBuilder;
        }

        public static IRequestBuilder AddQueryParam(this IRequestBuilder requestBuilder, string name, string value)
        {
            requestBuilder.Query.Add(name, value);
            return requestBuilder;
        }

        public static async Task<HttpResponseMessage> Execute(this IRequestBuilder requestBuilder, IOrcusRestClient client)
        {
            using (var request = requestBuilder.Build())
                return await client.SendMessage(request);
        }

        public static IEnumerable<KeyValuePair<string, string>> ToPairs(this NameValueCollection collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            return collection.Cast<string>().Select(key => new KeyValuePair<string, string>(key, collection[key]));
        }

        public static IRequestBuilder AddQuery(this IRequestBuilder requestBuilder, object query, string separator = ",")
        {
            var properties = query.GetType().GetProperties()
                .Where(x => x.CanRead)
                .Select(x => (x, x.GetValue(query, null)))
                .Where(x => x.Item1.PropertyType.IsValueType ? x.Item2 != Activator.CreateInstance(x.Item1.PropertyType) : x.Item2 != null)
                .ToDictionary(x => x.Item1.Name, x => x.Item2);

            // Get names for all IEnumerable properties (excl. string)
            var enumerableProperties = properties
                .Where(x => !(x.Value is string) && x.Value is IEnumerable)
                .Select(x => x.Key);

            // Concat all IEnumerable properties into a comma separated string
            foreach (var key in enumerableProperties)
            {
                var valueType = properties[key].GetType();
                var valueElemType = valueType.IsGenericType
                    ? valueType.GetGenericArguments()[0]
                    : valueType.GetElementType();
                if (valueElemType.IsPrimitive || valueElemType == typeof(string))
                {
                    var enumerable = properties[key] as IEnumerable;
                    properties[key] = string.Join(separator, enumerable.Cast<object>());
                }
            }

            foreach (var property in properties)
                requestBuilder.Query.Add(property.Key.ToCamelCase(), property.Value.ToString());
            return requestBuilder;
        }
    }
}
