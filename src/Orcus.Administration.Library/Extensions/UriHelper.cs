using System;

namespace Orcus.Administration.Library.Extensions
{
    public static class UriHelper
    {
        public static Uri CombineRelativeUris(Uri baseUri, Uri relativeUri) =>
            new Uri(baseUri.ToString().TrimEnd('/') + "/" + relativeUri.ToString().TrimStart('/'), UriKind.Relative);
    }
}