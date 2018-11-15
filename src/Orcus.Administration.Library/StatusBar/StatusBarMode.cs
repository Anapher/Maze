namespace Orcus.Administration.Library.StatusBar
{
    /// <summary>
    ///     The mode of the status bar when the message is displayed
    /// </summary>
    public enum StatusBarMode
    {
        /// <summary>
        ///     A default status message
        /// </summary>
        Default,

        /// <summary>
        ///     A warning message. The background color will change to orange.
        /// </summary>
        Warning,
        
        /// <summary>
        ///     An error message. The background color will change to red.
        /// </summary>
        Error,

        /// <summary>
        ///     A success message. The background color will change to green.
        /// </summary>
        Success
    }
}