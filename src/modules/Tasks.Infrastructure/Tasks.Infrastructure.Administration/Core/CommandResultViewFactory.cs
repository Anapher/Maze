using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Windows;
using Maze.Administration.Library.Logging;
using Tasks.Infrastructure.Administration.Library.Result;
using Tasks.Infrastructure.Administration.Utilities;
using Tasks.Infrastructure.Core.Dtos;
using Unity;

namespace Tasks.Infrastructure.Administration.Core
{
    public interface ICommandResultViewFactory
    {
        UIElement GetView(CommandResultDto commandResult, string result);
    }

    public class CommandResultViewFactory : ICommandResultViewFactory
    {
        private readonly IUnityContainer _context;
        private static readonly ILog Logger = LogProvider.For<CommandResultViewFactory>();

        public CommandResultViewFactory(IUnityContainer context)
        {
            _context = context;
        }

        public UIElement GetView(CommandResultDto commandResult, string result)
        {
            HttpResponseMessage response = null;
            if (commandResult.Status != null)
                response = HttpResponseDeserializer.DecodeResponse(result);

            using (var scope = _context.CreateChildContainer())
            {
                var viewProviders = scope.Resolve<IEnumerable<ICommandResultViewProvider>>().OrderByDescending(x => x.Priority);
                var serviceProvider = scope.Resolve<IServiceProvider>();

                foreach (var viewProvider in viewProviders)
                    try
                    {
                        var view = viewProvider.GetView(response, commandResult, serviceProvider);
                        if (view != null)
                            return view;
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e, "Error occurred on GetView() on {viewProviderType}", viewProvider.GetType().FullName);
                    }
            }

            return null;
        }
    }
}