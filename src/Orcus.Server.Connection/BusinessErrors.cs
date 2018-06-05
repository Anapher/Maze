using Orcus.Server.Connection.Error;

namespace Orcus.Server.Connection
{
    public static class BusinessErrors
    {
        public static RestError FieldNullOrEmpty(string name)
            => new RestError(ErrorTypes.ValidationError, $"The field {name} must not be null or empty.",
                (int) ErrorCode.FieldNullOrEmpty);

        public static RestError InvalidSha256Hash => CreateValidationError(
            "The value must be a valid SHA256 hash. A SHA256 hash consists of 64 hexadecimal characeters.",
            ErrorCode.InvalidSha256Hash);

        public static RestError InvalidMacAddress => CreateValidationError(
            "The value must be a valid mac address. A mac address must consist of 6 bytes.",
            ErrorCode.InvalidSha256Hash);

        public static RestError InvalidCultureName => CreateValidationError(
            "The given culture is not supported.", ErrorCode.InvalidCultureName);

        public static class Account
        {
            public static RestError InvalidJwt =>
                CreateAuthenticationError("The JSON Web Token is invalid.", ErrorCode.Account_InvalidJwt);

            public static RestError UsernameNotFound =>
                CreateAuthenticationError("The username was not found.", ErrorCode.Account_UsernameNotFound);

            public static RestError InvalidPassword =>
                CreateAuthenticationError("The password is invalid for this account.",
                    ErrorCode.Account_InvalidPassword);

            public static RestError AccountDisabled =>
                CreateAuthenticationError(
                    "The account was disabled. If you think this is an error, please contact our support.",
                    ErrorCode.Account_Disabled);

            private static RestError CreateAuthenticationError(string message, ErrorCode code)
            {
                return new RestError(ErrorTypes.AuthenticationError, message, (int) code);
            }
        }

        private static RestError CreateValidationError(string message, ErrorCode code) =>
            new RestError(ErrorTypes.ValidationError, message, (int) code);
    }
}