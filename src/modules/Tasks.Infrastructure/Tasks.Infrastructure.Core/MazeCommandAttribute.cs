using System;
using Tasks.Infrastructure.Core.Commands;

namespace Tasks.Infrastructure.Core
{
    /// <summary>
    ///     An attribute for a <see cref="CommandInfo"/> to specify the information
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class MazeCommandAttribute : Attribute
    {
        /// <summary>
        ///     The name of the command
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     The required modules separated by a semicolon ';'
        /// </summary>
        public string Modules { get; }

        /// <summary>
        ///     Create a new <see cref="MazeCommandAttribute"/>
        /// </summary>
        /// <param name="name">The name of the command</param>
        /// <param name="modules">The required modules separated by a semicolon ';'</param>
        public MazeCommandAttribute(string name, string modules)
        {
            Name = name;
            Modules = modules;
        }
    }
}