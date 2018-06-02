using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Orcus.Modules.Api;
using Orcus.Modules.Api.Parameters;
using Orcus.Server.Service.Commanding.ModelBinding;

namespace Orcus.Server.Service.Commanding
{
    public class ActionInvoker
    {
        public ActionInvoker(Type controllerType, MethodInfo routeMethod)
        {
            ControllerType = controllerType;
            RouteMethod = routeMethod;

            ObjectFactory = ActivatorUtilities.CreateFactory(ControllerType, new Type[0]);
            RouteMethodDelegate = BuildDelegate(routeMethod);

            var parameters = new List<ParameterDescriptor>();
            foreach (var parameter in RouteMethod.GetParameters())
            {
                var methodParam = new ParameterDescriptor
                {
                    Name = parameter.Name,
                    ParameterType = parameter.ParameterType,
                    BindingSource = BindingSource.ModelBinding
                };
                parameters.Add(methodParam);

                var attributes = parameter.GetCustomAttributes();
                foreach (var attribute in attributes)
                {
                    switch (attribute)
                    {
                        case FromBodyAttribute _:
                            methodParam.BindingSource = BindingSource.Body;
                            break;
                        case FromQueryAttribute fromQueryAttribute:
                            methodParam.BindingSource = BindingSource.Query;
                            methodParam.BinderModelName = fromQueryAttribute.Name;
                            break;
                        case FromServicesAttribute _:
                            methodParam.BindingSource = BindingSource.Services;
                            break;
                        case FromHeaderAttribute fromHeaderAttribute:
                            methodParam.BindingSource = BindingSource.Services;
                            methodParam.BinderModelName = fromHeaderAttribute.Name;
                            break;
                        default:
                            continue;
                    }

                    break;
                }
            }

            Parameters = parameters.ToImmutableList();
        }

        public delegate object MethodDelegate(object instance, object[] arguments);

        public ObjectFactory ObjectFactory { get; }
        public Type ControllerType { get; }
        public MethodInfo RouteMethod { get; }
        public MethodDelegate RouteMethodDelegate { get; }
        public IImmutableList<ParameterDescriptor> Parameters { get; }

        public async Task<object> Invoke(IServiceProvider serviceProvider, ActionContext actionContext)
        {
            var controller = (OrcusController) ObjectFactory(serviceProvider, new object[0]);
            controller.OrcusContext = actionContext.OrcusContext;

            var parameterBindingInfo = GetParameterBindingInfo(
                modelBinderFactory,
                modelMetadataProvider,
                actionDescriptor,
                mvcOptions);

            var parameters = new object[Parameters.Count];
            var parameterBinding = new ParameterBinder();

            for (var i = 0; i < Parameters.Count; i++)
            {
                var parameterDescriptor = Parameters[i];

                await parameterBinding.BindModelAsync(actionContext, null, null, parameterDescriptor, new ModelMetadata(), null);
            }
        }

        private BinderItem[] GetParameterBindingInfo(    IModelBinderFactory modelBinderFactory)
        {
            if (Parameters.Count == 0)
                return null;

            var parameterBindingInfo = new BinderItem[Parameters.Count];
            for (var i = 0; i < Parameters.Count; i++)
            {
                var parameter = Parameters[i];
                var metadata = new ModelMetadata(parameter);

                var binder = modelBinderFactory.CreateBinder(new ModelBinderFactoryContext
                {
                    BindingInfo = parameter.BindingInfo,
                    Metadata = metadata,
                    CacheToken = parameter,
                });

                parameterBindingInfo[i] = new BinderItem(binder, metadata);
            }

            return parameterBindingInfo;
        }

        private static MethodDelegate BuildDelegate(MethodInfo methodInfo)
        {
            var instanceExpression = Expression.Parameter(typeof(object), "instance");
            var argumentsExpression = Expression.Parameter(typeof(object[]), "arguments");
            var argumentExpressions = new List<Expression>();
            var parameterInfos = methodInfo.GetParameters();

            for (var i = 0; i < parameterInfos.Length; ++i)
            {
                var parameterInfo = parameterInfos[i];
                argumentExpressions.Add(Expression.Convert(
                    Expression.ArrayIndex(argumentsExpression, Expression.Constant(i)), parameterInfo.ParameterType));
            }

            var callExpression = Expression.Call(Expression.Convert(instanceExpression, methodInfo.ReflectedType),
                methodInfo, argumentExpressions);

            return Expression.Lambda<MethodDelegate>(Expression.Convert(callExpression, typeof(object)),
                instanceExpression, argumentsExpression).Compile();
        }
    }
}