using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Windows;
using Autofac;
using Microsoft.Extensions.Logging;
using Tasks.Infrastructure.Administration.Library.Result;
using Tasks.Infrastructure.Administration.Utilities;
using Tasks.Infrastructure.Core.Dtos;

namespace Tasks.Infrastructure.Administration.Core
{
    public interface ICommandResultViewFactory
    {
        UIElement GetView(CommandResultDto commandResult, string result);
    }

    public class CommandResultViewFactory : ICommandResultViewFactory
    {
        private readonly ILifetimeScope _context;
        private readonly ILogger<CommandResultViewFactory> _logger;

        public CommandResultViewFactory(ILifetimeScope context, ILogger<CommandResultViewFactory> logger)
        {
            _context = context;
            _logger = logger;
        }

        public UIElement GetView(CommandResultDto commandResult, string result)
        {
            HttpResponseMessage response = null;
            if (commandResult.Status != null)
                response = HttpResponseDeserializer.DecodeResponse(result);

            using (var scope = _context.BeginLifetimeScope())
            {
                var viewProviders = scope.Resolve<IEnumerable<ICommandResultViewProvider>>().OrderByDescending(x => x.Priority);

                foreach (var viewProvider in viewProviders)
                    try
                    {
                        var view = viewProvider.GetView(response, commandResult, _context);
                        if (view != null)
                            return view;
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error occurred on GetView() on {viewProviderType}", viewProvider.GetType().FullName);
                    }
            }

            return null;
        }
    }
}