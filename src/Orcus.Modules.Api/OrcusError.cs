using System;

namespace Orcus.Modules.Api
{
    public class OrcusError
    {
        public string Message { get; set; }
        public string StackTrace { get; set; }

        public static OrcusError FromException(Exception exception)
        {
            return new OrcusError {Message = exception.Message, StackTrace = exception.StackTrace};
        }
    }
}