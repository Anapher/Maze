// ReSharper disable InconsistentNaming
namespace Maze.Server.Connection.Error
{
    public enum ErrorCode
    {
        FieldNullOrEmpty = 0,
        InvalidSha256Hash,
        InvalidCultureName,

        Account_InvalidJwt = 1000,
        Account_UsernameNotFound,
        Account_InvalidPassword,
        Account_Disabled,

        Commander_ClientNotFound = 2000,
        Commander_SingleCommandTargetRequired,
        Commander_CommandTransmissionFailed,
        Commander_RouteNotFound,
        Commander_ActionError,
        Commander_ResultError,

        ClientGroups_NotFound = 3000,

        ClientConfigurations_InvalidJson = 4000,
        ClientConfigurations_CannotCreateGlobalConfig,
        ClientConfigurations_AlreadyExists,
        ClientConfigurations_NotFound,
    }
}