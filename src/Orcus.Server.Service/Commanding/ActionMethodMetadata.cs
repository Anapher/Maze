using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Orcus.Server.Service.Commanding
{
    public class ActionMethodMetadata
    {
        public Type ControllerType { get; }
        public MethodInfo MethodInfo { get; }

        public Type MethodReturnType { get; }
        public bool IsAsync { get; }

        public ActionMethodMetadata(Type controllerType, MethodInfo methodInfo)
        {
            ControllerType = controllerType;
            MethodInfo = methodInfo;

            MethodReturnType = methodInfo.ReturnType;
            IsAsync = typeof(Task).IsAssignableFrom(MethodReturnType);
        }
    }
}