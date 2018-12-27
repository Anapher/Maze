namespace Orcus.Server.Connection
{
    public class RestError
    {
        public RestError(string type, string message, int code)
        {
            Type = type;
            Message = message;
            Code = code;
        }

        public RestError()
        {
        }

        public string Type { get; set; }
        public string Message { get; set; }
        public int Code { get; set; }

        public static implicit operator RestErrorValidationResult(RestError error)
        {
            return new RestErrorValidationResult(error);
        }
    }
}