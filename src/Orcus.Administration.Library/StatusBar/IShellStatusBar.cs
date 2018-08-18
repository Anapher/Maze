namespace Orcus.Administration.Library.StatusBar
{
    public interface IShellStatusBar
    {
        void PushStatus(StatusMessage message);
    }

    public abstract class StatusMessage
    {
    }

    public class TextStatusMessage
    {

    }
}