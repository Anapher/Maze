using System.Collections.Generic;
using System.Net;

namespace Orcus.Server.Connection.Error
{
    public static class ErrorTypes
    {
        public const string ValidationError = "InvalidArgumentException";
        public const string AuthenticationError = "AuthenticationException";
        public const string NotFoundError = "NotFoundException";

        public static IReadOnlyDictionary<string, HttpStatusCode> ErrorStatusCodes { get; } =
            new Dictionary<string, HttpStatusCode>
            {
                {ValidationError, HttpStatusCode.BadRequest},
                {AuthenticationError, HttpStatusCode.BadRequest},
                {NotFoundError, HttpStatusCode.NotFound}
            };
    }
}
