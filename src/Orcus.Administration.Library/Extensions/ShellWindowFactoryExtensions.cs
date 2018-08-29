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

        public static void Show<TViewModel>(this IWindowService windowService, TViewModel viewModel,
            string titleResourceKey)
        {
            windowService.Show(viewModel, titleResourceKey, null, null, null);
        }

        public static void Show<TViewModel>(this IWindowService windowService, TViewModel viewModel,
            string titleResourceKey, IWindow owner)
        {
            windowService.Show(viewModel, titleResourceKey, owner, null, null);
        }

        public static void Show<TViewModel>(this IWindowService windowService)
        {
            windowService.Show(typeof(TViewModel), null, null, null, null);
        }

        public static void Show<TViewModel>(this IWindowService windowService, string titleResourceKey)
        {
            windowService.Show(typeof(TViewModel), titleResourceKey, null, null, null);
        }

        public static void Show<TViewModel>(this IWindowService windowService, string titleResourceKey, IWindow owner)
        {
            windowService.Show(typeof(TViewModel), titleResourceKey, owner, null, null);
        }

        public static bool? ShowDialog<TViewModel>(this IWindowService windowService, TViewModel viewModel) =>
            windowService.ShowDialog(viewModel, null, null, null, null);

        public static bool? ShowDialog<TViewModel>(this IWindowService windowService, TViewModel viewModel,
            string titleResourceKey) =>
            windowService.ShowDialog(viewModel, titleResourceKey, null, null, null);

        public static bool? ShowDialog<TViewModel>(this IWindowService windowService, TViewModel viewModel,
            string titleResourceKey, IWindow owner) =>
            windowService.ShowDialog(viewModel, titleResourceKey, owner, null, null);

        public static bool? ShowDialog<TViewModel>(this IWindowService windowService) =>
            windowService.ShowDialog(typeof(TViewModel), null, null, null, null);

        public static bool? ShowDialog<TViewModel>(this IWindowService windowService, string titleResourceKey) =>
            windowService.ShowDialog(typeof(TViewModel), titleResourceKey, null, null, null);

        public static bool? ShowDialog<TViewModel>(this IWindowService windowService, string titleResourceKey,
            IWindow owner) =>
            windowService.ShowDialog(typeof(TViewModel), titleResourceKey, owner, null, null);
    }
}