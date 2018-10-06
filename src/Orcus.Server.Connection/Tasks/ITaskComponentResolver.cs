using System;

namespace Orcus.Server.Connection.Tasks
{
    public interface ITaskComponentResolver
    {
        Type ResolveCondition(string name);
        Type ResolveTransmissionInfo(string name);
        Type ResolveExecutionInfo(string name);
        Type ResolveStopEvent(string name);
        Type ResolveCommand(string name);

        string ResolveName(Type type);
    }
}