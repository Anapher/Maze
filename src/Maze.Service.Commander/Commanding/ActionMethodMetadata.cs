using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Maze.Service.Commander.Commanding
{
    /// <summary>
    ///     Metadata of an action method
    /// </summary>
    public class ActionMethodMetadata
    {
        /// <summary>
        ///     Initialize a new instance of <see cref="ActionMethodMetadata"/>
        /// </summary>
        /// <param name="controllerType">The controller type</param>
        /// <param name="methodInfo">The method</param>
        public ActionMethodMetadata(Type controllerType, MethodInfo methodInfo)
        {
            //Debug.Assert(controllerType == methodInfo.DeclaringType);

            ControllerType = controllerType;
            MethodInfo = methodInfo;

            MethodReturnType = methodInfo.ReturnType;
            IsAsync = typeof(Task).IsAssignableFrom(MethodReturnType);

            if (IsAsync && MethodReturnType.IsGenericType)
            {
                AsyncResultType = MethodReturnType.GetGenericArguments().First();
            }
        }

        /// <summary>
        ///     The controller type
        /// </summary>
        public Type ControllerType { get; }

        /// <summary>
        ///     The method info
        /// </summary>
        public MethodInfo MethodInfo { get; }

        /// <summary>
        ///     The return type of the method
        /// </summary>
        public Type MethodReturnType { get; }

        /// <summary>
        ///     The actual return type if the method returns a task
        /// </summary>
        public Type AsyncResultType { get; }

        /// <summary>
        /// True if the method is asynchronous
        /// </summary>
        public bool IsAsync { get; }
    }
}