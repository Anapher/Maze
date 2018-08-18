namespace Orcus.Administration.Library.Extensions
{
    public class SuccessOrError<T>
    {
        public static SuccessOrError<T> DefaultFailed = new SuccessOrError<T>();

        public SuccessOrError(T result)
        {
            Result = result;
        }

        private SuccessOrError()
        {
            Failed = true;
        }

        public bool Failed { get; }
        public T Result { get; }

        public static implicit operator SuccessOrError<T>(T result) => new SuccessOrError<T>(result);
    }
}