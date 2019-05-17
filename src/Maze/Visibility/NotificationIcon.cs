using System.Windows.Forms;
using Maze.Properties;

namespace Maze.Visibility
{
    public class NotificationIcon
    {
        private NotifyIcon _trayIcon;

        public void InitializeComponent()
        {
            _trayIcon = new NotifyIcon {Icon = Resources.icon};
            _trayIcon.Visible = true;
            _trayIcon.Text = "Maze Client";
            _trayIcon.ShowBalloonTip(0, "Maze", "The Maze client is now running and trying to connect to the server", ToolTipIcon.Info);
        }

        public void ApplicationExit()
        {
            _trayIcon.Visible = false;
            _trayIcon.Dispose();
        }
    }
}