using System;
using MahApps.Metro.IconPacks;
using Prism;

namespace Orcus.Administration.Library.ViewModels
{
    public class OverviewTabBase : ViewModelBase, IActiveAware
    {
        private bool _isActive;
        private bool _isInitialized;

        public OverviewTabBase(string title, PackIconFontAwesomeKind icon)
        {
            Title = title;
            Icon = icon;
        }

        public string Title { get; }
        public PackIconFontAwesomeKind Icon { get; }

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;

                    if (value && !_isInitialized)
                    {
                        _isInitialized = true;
                        OnInitialize();
                    }

                    if (value)
                        OnActivated();
                    else OnDeactivated();
                }
            }
        }

        public event EventHandler IsActiveChanged;

        public virtual void OnActivated()
        {
        }

        public virtual void OnDeactivated()
        {
        }

        public virtual void OnInitialize()
        {
        }
    }
}