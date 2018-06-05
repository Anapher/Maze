using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace Orcus.Server.Service.Commanding
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
            Debug.Assert(controllerType == methodInfo.DeclaringType);

            ControllerType = controllerType;
            MethodInfo = methodInfo;

            MethodReturnType = methodInfo.ReturnType;
            IsAsync = typeof(Task).IsAssignableFrom(MethodReturnType);
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
        /// The return type of the method
        /// </summary>
        public Type MethodReturnType { get; }

        /// <summary>
        /// True if the method is asynchronous
        /// </summary>
        public bool IsAsync { get; }
    }
}