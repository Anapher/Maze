namespace Orcus.Administration.Library.Extensions
{
    /// <summary>
    ///     A result that either carries a success or an error
    /// </summary>
    /// <typeparam name="T">The return type.</typeparam>
    public class SuccessOrError<T>
    {
        /// <summary>
        ///     Get the failed instance for the <see cref="{T}"/>
        /// </summary>
        public static SuccessOrError<T> DefaultFailed = new SuccessOrError<T>();

        /// <summary>
        ///     Initialize a new successful <see cref="SuccessOrError{T}"/>
        /// </summary>
        /// <param name="result">The result</param>
        public SuccessOrError(T result)
        {
            Result = result;
        }

        private SuccessOrError()
        {
            Failed = true;
        }

        /// <summary>
        ///     Gets whether the process has failed
        /// </summary>
        public bool Failed { get; }

        /// <summary>
        ///     The result value. If <see cref="Failed"/> is <code>false</code>, this value is always null.
        /// </summary>
        public T Result { get; }

        /// <summary>
        ///     Convert any object to a successful <see cref="SuccessOrError{T}"/>
        /// </summary>
        /// <param name="result">The result.</param>
        public static implicit operator SuccessOrError<T>(T result) => new SuccessOrError<T>(result);
    }
}