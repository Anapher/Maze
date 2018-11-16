using System;

namespace Tasks.Infrastructure.Core
{
    /// <summary>
    ///     A resolver class to resolve the required task services
    /// </summary>
    public interface ITaskComponentResolver
    {
        /// <summary>
        ///     Resolve a filter by its name from the task file
        /// </summary>
        /// <param name="name">The name of the filter</param>
        /// <returns>Return the type of the filter</returns>
        Type ResolveFilter(string name);

        /// <summary>
        ///     Resolve a trigger by its name from the task file
        /// </summary>
        /// <param name="name">The name of the trigger</param>
        /// <returns>Return the type of the trigger</returns>
        Type ResolveTrigger(string name);

        /// <summary>
        ///     Resolve a stop event by its name from the task file
        /// </summary>
        /// <param name="name">The name of the stop event</param>
        /// <returns>Return the type of the stop event</returns>
        Type ResolveStopEvent(string name);

        /// <summary>
        ///     Resolve a command by its name from the task file
        /// </summary>
        /// <param name="name">The name of the command</param>
        /// <returns>Return the type of the command</returns>
        Type ResolveCommand(string name);

        /// <summary>
        ///     Resolve the key name of a task service type
        /// </summary>
        /// <param name="type">The type of the service</param>
        /// <returns>Return the task service name that will be found by a Resolve* function</returns>
        string ResolveName(Type type);
    }
}