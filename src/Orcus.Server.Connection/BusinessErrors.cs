using System;
using System.Collections.Generic;
using System.Text;
using Orcus.Server.Connection.Error;

namespace Orcus.Server.Connection
{
    public static class BusinessErrors
    {
        public static class Account
        {
            private static RestError CreateAuthenticationError(string message, ErrorCode code)
            {
                return new RestError(ErrorTypes.AuthenticationError, message, (int)code);
            }

            public static RestError InvalidJwt =>
                CreateAuthenticationError("The JSON Web Token is invalid.", ErrorCode.Account_InvalidJwt);

            public static RestError UsernameNotFound =>
                CreateAuthenticationError("The username was not found.", ErrorCode.Account_UsernameNotFound);

            public static RestError InvalidPassword =>
                CreateAuthenticationError("The password is invalid for this account.", ErrorCode.Account_InvalidPassword);

            public static RestError AccountDisabled =>
                CreateAuthenticationError(
                    "The account was disabled. If you think this is an error, please contact our support.",
                    ErrorCode.Account_Disabled);
        }
    }
}
