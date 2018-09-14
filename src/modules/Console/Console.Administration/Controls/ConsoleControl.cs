using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Console.Shared.Dtos;
using Unclassified.TxLib;

namespace Console.Administration.Controls
{
    /// <summary>
    ///     The Console Control allows you to embed a basic console in your application.
    /// </summary>
    public partial class ConsoleControl : UserControl
    {
        /// <summary>
        ///     Current position that input starts at.
        /// </summary>
        private int _inputStart = -1;

        /// <summary>
        ///     The is input enabled flag.
        /// </summary>
        private bool _isInputEnabled = true;

        /// <summary>
        ///     The last input string (used so that we can make sure we don't echo input twice).
        /// </summary>
        private string _lastInput;

        private IProcessConsole _processConsole;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConsoleControl" /> class.
        /// </summary>
        public ConsoleControl()
        {
            //  Initialise the component.
            InitializeComponent();

            //  Show diagnostics disabled by default.
            ShowDiagnostics = false;

            //  Input enabled by default.
            IsInputEnabled = true;

            //  Disable special commands by default.
            SendKeyboardCommandsToProcess = false;

            //  Wait for key down messages on the rich text box.
            InternalRichTextBox.KeyDown += richTextBoxConsole_KeyDown;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether to show diagnostics.
        /// </summary>
        /// <value>
        ///     <c>true</c> if show diagnostics; otherwise, <c>false</c>.
        /// </value>
        [Category("Console Control")]
        [Description("Show diagnostic information, such as exceptions.")]
        public bool ShowDiagnostics { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is input enabled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is input enabled; otherwise, <c>false</c>.
        /// </value>
        [Category("Console Control")]
        [Description("If true, the user can key in input.")]
        public bool IsInputEnabled
        {
            get => _isInputEnabled;
            set
            {
                _isInputEnabled = value;
                if (IsProcessRunning)
                    InternalRichTextBox.ReadOnly = !value;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether [send keyboard commands to process].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [send keyboard commands to process]; otherwise, <c>false</c>.
        /// </value>
        [Category("Console Control")]
        [Description("If true, special keyboard commands like Ctrl-C and tab are sent to the process.")]
        public bool SendKeyboardCommandsToProcess { get; set; }

        /// <summary>
        ///     Gets a value indicating whether this instance is process running.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is process running; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false)]
        public bool IsProcessRunning => ProcessConsole?.IsRunning ?? false;

        /// <summary>
        ///     Gets the internal rich text box.
        /// </summary>
        [Browsable(false)]
        public RichTextBox InternalRichTextBox { get; private set; }

        /// <summary>
        ///     Gets the process interface.
        /// </summary>
        [Browsable(false)]
        public IProcessConsole ProcessConsole
        {
            get => _processConsole;
            set
            {
                if (_processConsole != value)
                {
                    if (_processConsole != null)
                    {
                        _processConsole.Exited -= OnProcessExited;
                        _processConsole.Output -= OnOutput;
                        _processConsole.Error -= OnError;

                        ClearOutput();
                    }

                    _processConsole = value;

                    if (value != null)
                    {
                        value.Exited += OnProcessExited;
                        value.Output += OnOutput;
                        value.Error += OnError;

                        //  If we enable input, make the control not read only.
                        if (IsInputEnabled)
                            InternalRichTextBox.ReadOnly = false;
                    }
                    else
                    {
                        InternalRichTextBox.ReadOnly = true;
                    }
                }
            }
        }

        /// <summary>
        ///     Gets or sets the font of the text displayed by the control.
        /// </summary>
        /// <returns>
        ///     The <see cref="T:System.Drawing.Font" /> to apply to the text displayed by the control. The default is the
        ///     value of the <see cref="P:System.Windows.Forms.Control.DefaultFont" /> property.
        /// </returns>
        /// <PermissionSet>
        ///     <IPermission
        ///         class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
        ///         version="1" Unrestricted="true" />
        ///     <IPermission
        ///         class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
        ///         version="1" Unrestricted="true" />
        ///     <IPermission
        ///         class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
        ///         version="1" Flags="UnmanagedCode, ControlEvidence" />
        ///     <IPermission
        ///         class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
        ///         version="1" Unrestricted="true" />
        /// </PermissionSet>
        public override Font Font
        {
            get => base.Font;
            set
            {
                //  Set the base class font...
                base.Font = value;

                //  ...and the internal control font.
                InternalRichTextBox.Font = value;
            }
        }

        /// <summary>
        ///     Gets or sets the background color for the control.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.Drawing.Color" /> that represents the background color of the control. The default is
        ///     the value of the <see cref="P:System.Windows.Forms.Control.DefaultBackColor" /> property.
        /// </returns>
        /// <PermissionSet>
        ///     <IPermission
        ///         class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
        ///         version="1" Unrestricted="true" />
        /// </PermissionSet>
        public override Color BackColor
        {
            get => base.BackColor;
            set
            {
                //  Set the base class background...
                base.BackColor = value;

                //  ...and the internal control background.
                InternalRichTextBox.BackColor = value;
            }
        }

        /// <summary>
        ///     This event occurres once the user presses ctrl+c
        /// </summary>
        public event EventHandler ExitRequested;

        private void OnError(object sender, ProcessOutputEventArgs e)
        {
            //  Write the output, in red
            WriteOutput(e.Content, Color.Red);
        }

        private void OnOutput(object sender, ProcessOutputEventArgs e)
        {
            //  Write the output, in white
            WriteOutput(e.Content, Color.White);
        }

        /// <summary>
        ///     Handles the OnProcessExit event of the processInterace control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="ProcessEventArgs" /> instance containing the event data.</param>
        private void OnProcessExited(object sender, ProcessExitedEventArgs e)
        {
            //  Are we showing diagnostics?
            if (ShowDiagnostics)
                WriteOutput(Environment.NewLine + Tx.T("Console:ExitLine", "name", ProcessConsole.Filename), Color.FromArgb(255, 0, 255, 0));

            if (!IsHandleCreated)
                return;

            //  Read only again.
            Invoke((Action) (() => InternalRichTextBox.ReadOnly = true));
        }

        /// <summary>
        ///     Handles the KeyDown event of the richTextBoxConsole control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs" /> instance containing the event data.</param>
        private void richTextBoxConsole_KeyDown(object sender, KeyEventArgs e)
        {
            //  Are we sending keyboard commands to the process?
            if (IsProcessRunning)
                if (e.Control && e.KeyCode == Keys.C)
                {
                    ExitRequested?.Invoke(this, EventArgs.Empty);

                    e.SuppressKeyPress = true;
                    return;
                }

            //  If we're at the input point and it's backspace, bail.
            if (InternalRichTextBox.SelectionStart <= _inputStart && e.KeyCode == Keys.Back) e.SuppressKeyPress = true;

            //  Are we in the read-only zone?
            if (InternalRichTextBox.SelectionStart < _inputStart)
                if (!(e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.Up ||
                      e.KeyCode == Keys.Down || e.KeyCode == Keys.C && e.Control))
                    e.SuppressKeyPress = true;

            //  Is it the return key?
            if (e.KeyCode == Keys.Return)
            {
                var length = InternalRichTextBox.SelectionStart - _inputStart;
                if (length <= 0)
                    return;

                if (_inputStart == -1)
                    return;

                //  Get the input.
                var input = InternalRichTextBox.Text.Substring(_inputStart, length);

                //  Write the input (without echoing).
                WriteInput(input, Color.White, false);
            }
        }

        /// <summary>
        ///     Writes the output to the console control.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="color">The color.</param>
        public void WriteOutput(string output, Color color)
        {
            if (string.IsNullOrEmpty(_lastInput) == false &&
                (output == _lastInput || output.Replace("\r\n", "") == _lastInput))
                return;

            if (!IsHandleCreated)
                return;

            Invoke((Action) (() =>
            {
                //  Write the output.
                InternalRichTextBox.SelectionColor = color;
                InternalRichTextBox.SelectedText += output;
                _inputStart = InternalRichTextBox.SelectionStart;
            }));
        }

        /// <summary>
        ///     Clears the output.
        /// </summary>
        public void ClearOutput()
        {
            InternalRichTextBox.Clear();
            _inputStart = 0;
        }

        /// <summary>
        ///     Writes the input to the console control.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="color">The color.</param>
        /// <param name="echo">if set to <c>true</c> echo the input.</param>
        public void WriteInput(string input, Color color, bool echo)
        {
            Invoke((Action) (() =>
            {
                //  Are we echoing?
                if (echo)
                {
                    InternalRichTextBox.SelectionColor = color;
                    InternalRichTextBox.SelectedText += input;
                    _inputStart = InternalRichTextBox.SelectionStart;
                }

                _lastInput = input;

                //  Write the input.
                ProcessConsole.WriteInput(input);
            }));
        }
    }
}