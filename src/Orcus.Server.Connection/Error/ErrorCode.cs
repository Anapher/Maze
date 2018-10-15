// ReSharper disable InconsistentNaming
namespace Orcus.Server.Connection.Error
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

        Tasks_NotFound = 3000,
        Tasks_TaskIdNull,
        Tasks_RestartOnFailIntervalMustBePositive,
        Tasks_MaximumRestartsMustBeGreaterThanZero,
        Tasks_NoAudienceGiven,
        Tasks_NoTriggersGiven,
        Tasks_NoCommandsGiven,
    }
}