using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Orcus.Modules.Api;
using Orcus.Modules.Api.Parameters;
using Orcus.Server.Service.Commanding.ModelBinding;

namespace Orcus.Server.Service.Commanding
{
    /// <summary>
    ///     Cached delegate for the controller method
    /// </summary>
    public class ActionInvoker
    {
        private readonly ObjectMethodExecutor _objectMethodExecutor;
        private readonly ActionMethodExecutor _executor;

        /// <summary>
        /// Initialize a new instance of <see cref="ActionInvoker"/>
        /// </summary>
        /// <param name="controllerType">The controller type</param>
        /// <param name="routeMethod">The action method that should be executed</param>
        public ActionInvoker(Type controllerType, MethodInfo routeMethod)
        {
            Metadata = new ActionMethodMetadata(controllerType, routeMethod);

            ObjectFactory = ActivatorUtilities.CreateFactory(controllerType, new Type[0]);
            _objectMethodExecutor = new ObjectMethodExecutor(routeMethod);
            _executor = ActionMethodExecutor.GetExecutor(Metadata);

            var parameters = new List<ParameterDescriptor>();
            foreach (var parameter in routeMethod.GetParameters())
            {
                var methodParam = new ParameterDescriptor
                {
                    Name = parameter.Name,
                    ParameterType = parameter.ParameterType,
                    BindingInfo = GetBindingInfo(parameter)
                };
                parameters.Add(methodParam);
            }

            Parameters = parameters.ToImmutableList();
        }

        public ActionMethodMetadata Metadata { get; }
        public ObjectFactory ObjectFactory { get; }
        public IImmutableList<ParameterDescriptor> Parameters { get; }

        /// <summary>
        /// Invoke the method with the given <see cref="ActionContext"/>
        /// </summary>
        /// <param name="actionContext">The action context that provides the information used to execute the method</param>
        /// <returns>Return the action result of the method</returns>
        public async Task<IActionResult> Invoke(ActionContext actionContext)
        {
            var controller = (OrcusController) ObjectFactory.Invoke(actionContext.Context.RequestServices, new object[0]);
            controller.OrcusContext = actionContext.Context;

            var parameterBindingInfo =
                GetParameterBindingInfo(actionContext.Context.RequestServices.GetRequiredService<IModelBinderFactory>());

            var arguments = new object[Parameters.Count];
            var parameterBinding = new ParameterBinder();

            for (var i = 0; i < Parameters.Count; i++)
            {
                var parameterDescriptor = Parameters[i];
                var bindingInfo = parameterBindingInfo[i];

                var result = await parameterBinding.BindModelAsync(actionContext, bindingInfo.ModelBinder, null, parameterDescriptor,
                    bindingInfo.ModelMetadata, value: null);

                if (result.IsModelSet)
                    arguments[i] = result.Model;
            }

            var actionResult = await _executor.Execute(_objectMethodExecutor, controller, arguments, Metadata);
            controller.Dispose();
            return actionResult;
        }

        private static BindingInfo GetBindingInfo(ParameterInfo parameter)
        {
            var attributes = parameter.GetCustomAttributes().ToList();
            var result = new BindingInfo();
            BindingSource? bindingSource = null;

            foreach (var sourceMetadata in attributes.OfType<IBindingSourceMetadata>())
            {
                bindingSource = sourceMetadata.BindingSource;
                break;
            }

            foreach (var modelNameProvider in attributes.OfType<IModelNameProvider>())
            {
                if (modelNameProvider.Name != null)
                {
                    result.BinderModelName = modelNameProvider.Name;
                    break;
                }
            }

            if (bindingSource == null)
            {
                //TODO correct default
                bindingSource = BindingSource.Header;
            }

            result.BindingSource = bindingSource.Value;
            return result;
        }

        private BinderItem[] GetParameterBindingInfo(IModelBinderFactory modelBinderFactory)
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
                    CacheToken = parameter
                });

                parameterBindingInfo[i] = new BinderItem(binder, metadata);
            }

            return parameterBindingInfo;
        }
        
        private struct BinderItem
        {
            public BinderItem(IModelBinder modelBinder, ModelMetadata modelMetadata)
            {
                ModelBinder = modelBinder;
                ModelMetadata = modelMetadata;
            }

            public IModelBinder ModelBinder { get; }
            public ModelMetadata ModelMetadata { get; }
        }
    }
}