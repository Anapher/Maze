namespace Orcus.Administration.Library.StatusBar
{
    //https://docs.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.shell.interop.ivsstatusbar?view=visualstudiosdk-2017
    public interface IShellStatusBar
    {
        TMessage PushStatus<TMessage>(TMessage message) where TMessage : StatusMessage;
    }
}