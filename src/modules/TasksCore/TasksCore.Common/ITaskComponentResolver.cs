using System;

namespace Orcus.Server.Connection.Tasks
{
    public interface ITaskComponentResolver
    {
        Type ResolveFilter(string name);
        Type ResolveTrigger(string name);
        Type ResolveStopEvent(string name);
        Type ResolveCommand(string name);

        string ResolveName(Type type);
    }
}