using Orcus.Server.Connection.Error;

namespace Orcus.Server.Connection
{
    public static class BusinessErrors
    {
        public static RestError FieldNullOrEmpty(string name)
        {
            return new RestError(ErrorTypes.ValidationError, $"The field {name} must not be null or empty.",
                (int) ErrorCode.FieldNullOrEmpty);
        }

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
    }
}