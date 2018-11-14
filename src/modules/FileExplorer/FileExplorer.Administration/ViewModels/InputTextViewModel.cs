using System;
using Prism.Commands;
using Prism.Mvvm;

namespace FileExplorer.Administration.ViewModels
{
    public class InputTextViewModel : BindableBase
    {
        private bool? _dialogResult;
        private DelegateCommand<string> _okCommand;
        private string _text;

        public InputTextViewModel()
        {
            Predicate = s => !string.IsNullOrEmpty(s);
        }

        public Predicate<string> Predicate { get; set; }
        public string Watermark { get; private set; }
        public string AffirmerButtonText { get; private set; }

        public bool? DialogResult
        {
            get => _dialogResult;
            set => SetProperty(ref _dialogResult, value);
        }

        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        public DelegateCommand<string> OkCommand
        {
            get
            {
                return _okCommand ?? (_okCommand =
                           new DelegateCommand<string>(parameter => DialogResult = true,
                               s => Predicate?.Invoke(s) ?? true).ObservesProperty(() => Text));
            }
        }

        public void Initialize(string defaultText, string watermark, string affirmerButtonText)
        {
            Text = defaultText;
            Watermark = watermark;
            AffirmerButtonText = affirmerButtonText;
        }
    }
}