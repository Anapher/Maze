using System.ComponentModel.DataAnnotations;
using Tasks.Infrastructure.Administration.Library;
using Tasks.Infrastructure.Administration.Library.Command;
using Tasks.Infrastructure.Administration.PropertyGrid;
using Tasks.Infrastructure.Administration.PropertyGrid.Attributes;
using Tasks.Infrastructure.Core;
using TrollCommands.Shared.Commands;
using Unclassified.TxLib;

namespace TrollCommands.Administration.Commands
{
    public class CrazyCursorViewModel : PropertyGridViewModel, ICommandViewModel<CrazyCursorCommandInfo>
    {
        public CrazyCursorViewModel()
        {
            this.RegisterPropertyByConvention(() => Delay, "TrollCommands:Commands.CrazyCursor.Properties.Interval");
            this.RegisterPropertyByConvention(() => Power, "TrollCommands:Commands.CrazyCursor.Properties.Power");
        }

        [NumericValue(Minimum = 0, StringFormat = "0 ms")]
        public int Delay { get; set; } = 100;

        [NumericValue(Minimum = 5)]
        public int Power { get; set; } = 20;

        public void Initialize(CrazyCursorCommandInfo model)
        {
            Delay = model.Delay;
            Power = model.Power;

            OnPropertiesChanged();
        }

        public ValidationResult ValidateInput() => ValidationResult.Success;
        public ValidationResult ValidateContext(MazeTask mazeTask) => ValidationResult.Success;

        public CrazyCursorCommandInfo Build()
        {
            return new CrazyCursorCommandInfo{Delay = Delay, Power = Power};
        }
    }
}