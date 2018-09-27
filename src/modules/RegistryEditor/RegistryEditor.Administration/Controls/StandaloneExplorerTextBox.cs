using System;
using System.Windows.Input;
using RegistryEditor.Administration.Extensions;
using TreeViewEx.Controls;

namespace RegistryEditor.Administration.Controls
{
    public class StandaloneExplorerTextBox : ExplorerTextBox
    {
        public StandaloneExplorerTextBox()
        {
            this.AddValueChanged(CurrentPathProperty, CurrentPathChanged);
        }

        private void CurrentPathChanged(object sender, EventArgs eventArgs)
        {
            Text = CurrentPath;
        }

        protected override void OnFocusLost()
        {
            base.OnFocusLost();
            IsInEditMode = false;
        }

        protected override void OnWantToLooseFocus()
        {
            base.OnWantToLooseFocus();
            MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);
            IsInEditMode = true;
        }
    }
}