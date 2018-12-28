using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Tasks.Infrastructure.Administration.ViewModels.CreateTask.Base;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Core.Audience;

namespace Tasks.Infrastructure.Administration.ViewModels.CreateTask
{
    public class AudienceViewModel : ITaskConfiguringViewModel
    {
        private readonly AudienceStatusChildViewModel _audienceStatus;

        public AudienceViewModel()
        {
            _audienceStatus = new AudienceStatusChildViewModel {NodeViewModel = this};
            Childs = new List<AudienceStatusChildViewModel> {_audienceStatus};
        }

        public List<AudienceStatusChildViewModel> Childs { get; }

        public bool IsSelected { get; set; }
        public object NodeViewModel => this;

        public void Initialize(MazeTask mazeTask)
        {
        }

        public IEnumerable<ValidationResult> ValidateInput() => Enumerable.Empty<ValidationResult>();

        public IEnumerable<ValidationResult> ValidateContext(MazeTask mazeTask) => Enumerable.Empty<ValidationResult>();

        public void Apply(MazeTask mazeTask)
        {
            mazeTask.Audience = new AudienceCollection {IsAll = true};
        }
    }
}