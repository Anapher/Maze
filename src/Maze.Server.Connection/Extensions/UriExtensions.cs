//Source: https://github.com/poulfoged/UriExtend (MIT)
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Orcus.Server.Connection.Extensions
{
    public static class UriExtensions
    {
        private static readonly Regex QueryPart = new Regex(@"[^\?#]*\??([^#]*)", RegexOptions.Compiled);

        /// <summary>
        ///     Adds the specified parameter to the Query String.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="paramName">Name of the parameter to add.</param>
        /// <param name="paramValue">Value for the parameter to add.</param>
        /// <returns>Url with added parameter.</returns>
        public static Uri AddQueryParameters(this Uri uri, string paramName, string paramValue)
        {
            var query = paramName + "=" + paramValue;

            if (uri.IsAbsoluteUri)
            {
                var uriBuilder = new UriBuilder(uri)
                {
                    Port = uri.Authority.EndsWith(uri.Port.ToString(CultureInfo.InvariantCulture)) ? uri.Port : -1
                };
                if (string.IsNullOrWhiteSpace(uriBuilder.Query))
                    uriBuilder.Query = query;
                else
                    uriBuilder.Query = string.Format("{0}&{1}", uriBuilder.Query.Substring(1), query);

                return uriBuilder.Uri;
            }

            var uriString = QueryPart.Replace(uri.ToString(),
                match => match.Value.Contains("?") ? match.Value + "&" + query : match.Value + "?" + query, 1);
            return new Uri(uriString, UriKind.Relative);
        }
    }
}