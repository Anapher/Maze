using System;

namespace Orcus.Administration.Modules.Models
{
    /// <summary>
    /// Feed specific state of current search operation to retrieve search results.
    /// Used for polling results of prolonged search. Opaque to external consumer.
    /// </summary>
    public class RefreshToken
    {
        public TimeSpan RetryAfter { get; set; }
    }
}