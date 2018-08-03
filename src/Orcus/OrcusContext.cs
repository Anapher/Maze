using Autofac;
using Autofac.Core;

namespace Orcus
{
    public class OrcusContext
    {
        public OrcusContext()
        {
            var builder = new ContainerBuilder();
            var container = builder.Build();

        }
    }
}