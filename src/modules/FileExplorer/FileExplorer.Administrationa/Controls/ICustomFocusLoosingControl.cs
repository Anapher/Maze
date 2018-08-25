using System;

namespace FileExplorer.Administration.Controls
{
    public interface ICustomFocusLoosingControl
    {
        event EventHandler FocusLost;
    }
}