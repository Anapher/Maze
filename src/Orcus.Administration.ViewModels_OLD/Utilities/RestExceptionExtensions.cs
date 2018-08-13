using System;
using Anapher.Wpf.Swan.ViewInterface;
using Orcus.Administration.Core.Exceptions;
using Orcus.Server.Connection.Error;
using Unclassified.TxLib;

namespace Orcus.Administration.ViewModels.Utilities
{
    public static class RestExceptionExtensions
    {
        public static void ShowMessageBox(this Exception exception)
        {
            if (exception is RestException restException)
                restException.ShowMessageBox();
            else
                WindowServiceInterface.Current.ShowErrorMessage(Tx.T("Exceptions:UnexpectedErrorOccurred", "message",
                    exception.Message));
        }

        public static void ShowMessageBox(this RestException exception)
        {
            WindowServiceInterface.Current.ShowErrorMessage(GetRestExceptionMessage(exception));
        }

        public static string GetRestExceptionMessage(this RestException exception)
        {
            var keyName = "Exceptions:RestErrors." + (ErrorCode) exception.ErrorId;
            var result = Tx.T(keyName);
            if (string.IsNullOrEmpty(result) || keyName == result.Trim('[', ']'))
                return exception.Message;

            return result;
        }
    }
}