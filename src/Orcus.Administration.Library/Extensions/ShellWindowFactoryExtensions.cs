using Anapher.Wpf.Swan.ViewInterface;
using Orcus.Administration.Library.Services;

namespace Orcus.Administration.Library.Extensions
{
    public static class ShellWindowFactoryExtensions
    {
        public static void Show<TViewModel>(this IWindowService windowService, TViewModel viewModel)
        {
            windowService.Show(viewModel, null, null, null, null);
        }

        public static void Show<TViewModel>(this IWindowService windowService, TViewModel viewModel, string title)
        {
            windowService.Show(viewModel, title, null, null, null);
        }

        public static void Show<TViewModel>(this IWindowService windowService, TViewModel viewModel, string title,
            IWindow owner)
        {
            windowService.Show(viewModel, title, owner, null, null);
        }

        public static void Show<TViewModel>(this IWindowService windowService)
        {
            windowService.Show(typeof(TViewModel), null, null, null, null);
        }

        public static void Show<TViewModel>(this IWindowService windowService, string title)
        {
            windowService.Show(typeof(TViewModel), title, null, null, null);
        }

        public static void Show<TViewModel>(this IWindowService windowService, string title, IWindow owner)
        {
            windowService.Show(typeof(TViewModel), title, owner, null, null);
        }

        public static bool? ShowDialog<TViewModel>(this IWindowService windowService, TViewModel viewModel) =>
            windowService.ShowDialog(viewModel, null, null, null, null);

        public static bool? ShowDialog<TViewModel>(this IWindowService windowService, TViewModel viewModel,
            string title) =>
            windowService.ShowDialog(viewModel, title, null, null, null);

        public static bool? ShowDialog<TViewModel>(this IWindowService windowService, TViewModel viewModel,
            string title, IWindow owner) =>
            windowService.ShowDialog(viewModel, title, owner, null, null);

        public static bool? ShowDialog<TViewModel>(this IWindowService windowService) =>
            windowService.ShowDialog(typeof(TViewModel), null, null, null, null);

        public static bool? ShowDialog<TViewModel>(this IWindowService windowService, string title) =>
            windowService.ShowDialog(typeof(TViewModel), title, null, null, null);

        public static bool? ShowDialog<TViewModel>(this IWindowService windowService, string title, IWindow owner) =>
            windowService.ShowDialog(typeof(TViewModel), title, owner, null, null);
    }
}