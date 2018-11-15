using System.Windows;
using System.Windows.Media;
using Anapher.Wpf.Swan.ViewInterface;
using MahApps.Metro.Controls;
using Orcus.Administration.Library.StatusBar;

namespace Orcus.Administration.Library.Views
{
    /// <summary>
    ///     The shell window
    /// </summary>
    public interface IShellWindow : IWindow
    {
        /// <summary>
        ///     Get/set the title of the window
        /// </summary>
        string Title { get; set; }

        /// <summary>
        ///     Get/set the content of the right side of the status bar
        /// </summary>
        object RightStatusBarContent { get; set; }

        /// <summary>
        ///     Get/set whether the escape key should close the window
        /// </summary>
        bool EscapeClosesWindow { get; set; }

        /// <summary>
        ///     Get/set the dialog result
        /// </summary>
        bool? DialogResult { get; set; }

        /// <summary>
        ///    Gets or sets a value that indicates whether the window has a task bar button.
        /// </summary>
        bool ShowInTaskbar { get; set; }

        /// <summary>
        /// Gets/sets the left window commands that hosts the user commands.
        /// </summary>
        WindowCommands LeftWindowCommands { get; set; }

        /// <summary>
        /// Gets/sets the right window commands that hosts the user commands.
        /// </summary>
        WindowCommands RightWindowCommands { get; set; }

        /// <summary>
        ///     Gets or sets a window's icon. This property can be set to a WPF control
        /// </summary>
        object TitleBarIcon { get; set; }

        /// <summary>
        ///     Gets or sets the task bar icon. If <see cref="TitleBarIcon"/> is null, the title bar icon will also be set to the value of this property
        /// </summary>
        ImageSource TaskBarIcon { get; set; }

        /// <summary>
        ///     Gets/sets the FlyoutsControl that hosts the window's flyouts.
        /// </summary>
        FlyoutsControl Flyouts { get; set; }

        /// <summary>
        ///     Gets or sets the resize mode.
        /// </summary>
        ResizeMode ResizeMode { get; set; }

        /// <summary>
        /// Gets or sets the suggested height of the element.
        /// </summary>
        double Height { get; set; }

        /// <summary>
        /// Gets or sets the suggested width of the element.
        /// </summary>
        double Width { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether a window will automatically size itself to fit the size of its content.
        /// </summary>
        SizeToContent SizeToContent { get; set; }

        /// <summary>
        ///     Initialize the content of the shell window.
        /// </summary>
        /// <param name="content">The content view control.</param>
        /// <param name="statusBarManager">The status bar manager. If this value is <code>null</code>, no status bar will be created.</param>
        void InitalizeContent(object content, StatusBarManager statusBarManager);

        /// <summary>
        ///     Opens the window and returns without waiting for the newly opened window to close.
        /// </summary>
        /// <param name="owner">The <see cref="IWindow"/> that owns this <see cref="IShellWindow"/></param>
        void Show(IWindow owner);

        /// <summary>
        /// 	Opens a window and returns only when the newly opened window is closed.
        /// </summary>
        /// <param name="owner">The <see cref="IWindow"/> that owns this <see cref="IShellWindow"/></param>
        /// <returns>A <see cref="Nullable{T}"/> value of type Boolean that specifies whether the activity was accepted (<code>true</code>) or canceled (<code>false</code>). The return value is the value of the <see cref="DialogResult"/> property before a window closes.</returns>
        bool? ShowDialog(IWindow owner);
    }
}