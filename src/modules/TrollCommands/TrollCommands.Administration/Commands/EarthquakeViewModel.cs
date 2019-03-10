using System.ComponentModel.DataAnnotations;
using Tasks.Infrastructure.Administration.Library;
using Tasks.Infrastructure.Administration.Library.Command;
using Tasks.Infrastructure.Administration.PropertyGrid;
using Tasks.Infrastructure.Administration.PropertyGrid.Attributes;
using Tasks.Infrastructure.Core;
using TrollCommands.Shared.Commands;

namespace TrollCommands.Administration.Commands
{
    public class EarthquakeViewModel : PropertyGridViewModel, ICommandViewModel<EarthquakeCommandInfo>
    {
        public EarthquakeViewModel()
        {
            this.RegisterPropertyByConvention(() => Interval, "TrollCommands:Commands.Earthquake.Properties.Interval");
            this.RegisterPropertyByConvention(() => Power, "TrollCommands:Commands.Earthquake.Properties.Power");
        }

        [NumericValue(Minimum = 0, StringFormat = "0 ms")]
        public int Interval { get; set; }

        [NumericValue(Minimum = 1)]
        public int Power { get; set; }

        public void Initialize(EarthquakeCommandInfo model)
        {
            Interval = model.Interval;
            Power = model.Power;

            OnPropertiesChanged();
        }

        public ValidationResult ValidateInput() => ValidationResult.Success;

        public ValidationResult ValidateContext(MazeTask mazeTask) => ValidationResult.Success;

        public EarthquakeCommandInfo Build() => new EarthquakeCommandInfo {Interval = Interval, Power = Power};
    }
}