using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet.Frameworks;

namespace Orcus.Core
{
    public interface IApplicationInfo
    {
        NuGetFramework Framework { get; }
    }
}
