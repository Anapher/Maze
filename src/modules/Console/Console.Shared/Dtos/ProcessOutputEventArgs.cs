namespace Console.Shared.Dtos
{
    public class ProcessOutputEventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ProcessOutputEventArgs" /> class.
        /// </summary>
        public ProcessOutputEventArgs()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProcessOutputEventArgs" /> class.
        /// </summary>
        /// <param name="content">The content.</param>
        public ProcessOutputEventArgs(string content)
        {
            //  Set the content and code.
            Content = content;
        }

        public string Content { get; set; }
    }
}