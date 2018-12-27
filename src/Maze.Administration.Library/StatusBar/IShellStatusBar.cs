namespace Orcus.Administration.Library.StatusBar
{
    //https://docs.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.shell.interop.ivsstatusbar?view=visualstudiosdk-2017
    /// <summary>
    ///     Provides functionality of the window status bar
    /// </summary>
    public interface IShellStatusBar
    {
        /// <summary>
        ///     Push a new status to the stack that will be immediately displayed.
        /// </summary>
        /// <typeparam name="TMessage">The type of the status message.</typeparam>
        /// <param name="message">The status message that should be displayed</param>
        /// <returns>Return the status <see cref="message"/> that must be disposed in order to remove the status from the stack.</returns>
        TMessage PushStatus<TMessage>(TMessage message) where TMessage : StatusMessage;
    }
}