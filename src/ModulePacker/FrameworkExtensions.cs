using System;
using NuGet.Frameworks;

namespace ModulePacker
{
    public static class FrameworkExtensions
    {
        public static NuGetFramework ToNuGetFramework(this OrcusFramework orcusFramework)
        {
            switch (orcusFramework)
            {
                case OrcusFramework.Administration:
                    return FrameworkConstants.CommonFrameworks.OrcusAdministration10;
                case OrcusFramework.Server:
                    return FrameworkConstants.CommonFrameworks.OrcusServer10;
                case OrcusFramework.Client:
                    return FrameworkConstants.CommonFrameworks.OrcusClient10;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orcusFramework), orcusFramework, null);
            }
        }
    }
}