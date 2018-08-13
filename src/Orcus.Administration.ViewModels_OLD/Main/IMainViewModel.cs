using System;

namespace Orcus.Administration.ViewModels.Main
{
    public interface IMainViewModel
    {
        event EventHandler<IMainViewModel> ShowView;

        void LoadViewModel();
        void UnloadViewModel();
    }
}