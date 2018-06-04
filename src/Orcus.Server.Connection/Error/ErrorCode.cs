// ReSharper disable InconsistentNaming
namespace Orcus.Server.Connection.Error
{
    public enum ErrorCode
    {
        FieldNullOrEmpty = 0,

        Account_InvalidJwt = 1000,
        Account_UsernameNotFound,
        Account_InvalidPassword,
        Account_Disabled
    }
}