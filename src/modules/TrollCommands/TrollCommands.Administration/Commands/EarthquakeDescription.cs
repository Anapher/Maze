using System;
using System.Windows;
using Tasks.Infrastructure.Administration.Library.Command;
using TrollCommands.Shared.Commands;
using Unclassified.TxLib;

namespace TrollCommands.Administration.Commands
{
    public class EarthquakeDescription : ICommandDescription
    {
        public string Name { get; } = Tx.T("TrollCommands:Commands.Earthquake");
        public string Namespace { get; } = PrismModule.CommandsNamespace;
        public string Summary { get; } = Tx.T("TrollCommands:Commands.Earthquake.Summary");
        public UIElement Icon { get; }
        public Type DtoType { get; } = typeof(EarthquakeCommandInfo);
    }
}