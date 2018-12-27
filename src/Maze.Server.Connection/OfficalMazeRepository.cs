using System;

namespace Orcus.Server.Connection
{
    public static class OfficalOrcusRepository
    {
        public static Uri Uri { get; } = new Uri("https://api.nuget.org/v3/index.json");
    }
}