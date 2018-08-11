using System.Collections.Generic;

namespace Orcus.Sockets.Internal
{
    public class HeadersHelper
    {
        private static readonly ISet<string> ContentHeaders;

        static HeadersHelper()
        {
            ContentHeaders = new HashSet<string>();
            AddKnownHeaders(ContentHeaders);
        }

        public static bool IsContentHeader(string name)
        {
            return ContentHeaders.Contains(name);
        }

        internal static void AddKnownHeaders(ISet<string> headerSet)
        {
            headerSet.Add("Allow");
            headerSet.Add("Content-Disposition");
            headerSet.Add("Content-Encoding");
            headerSet.Add("Content-Language");
            headerSet.Add("Content-Length");
            headerSet.Add("Content-Location");
            headerSet.Add("Content-MD5");
            headerSet.Add("Content-Range");
            headerSet.Add("Content-Type");
            headerSet.Add("Expires");
            headerSet.Add("Last-Modified");
        }
    }
}