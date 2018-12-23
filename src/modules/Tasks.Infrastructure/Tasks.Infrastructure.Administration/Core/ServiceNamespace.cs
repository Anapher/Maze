using System.Collections.Generic;
using System.Linq;
using Unclassified.TxLib;

namespace Tasks.Infrastructure.Administration.Core
{
    public class ServiceNamespace
    {
        public ServiceNamespace(IEnumerable<string> segments)
        {
            Segments = segments.ToList();
        }

        public ServiceNamespace()
        {
            Segments = new List<string>();
        }

        public List<string> Segments { get; }

        public IEnumerable<string> GetRelativeSegments()
        {
            for (int i = 0; i < Segments.Count; i++)
            {
                yield return string.Join(".", Segments.Take(i + 1));
            }
        }

        public static ServiceNamespace Parse(string serviceNamespace)
        {
            if (string.IsNullOrEmpty(serviceNamespace))
                return new ServiceNamespace();

            var segments = serviceNamespace.Split('.').ToList();
            return new ServiceNamespace(segments);
        }

        public static string TranslateSegment(string relativePath)
        {
            var result = Tx.T($"Tasks.Namespace.{relativePath}");
            if (result.StartsWith("[") && result.EndsWith("]"))
                return Parse(relativePath).Segments.Last();

            return result;
        }

        public override string ToString()
        {
            if (!Segments.Any())
                return null;

            return string.Join(".", Segments);
        }
    }
}
