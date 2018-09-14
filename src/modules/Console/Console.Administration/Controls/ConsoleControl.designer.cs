namespace Console.Administration.Controls
{
  partial class ConsoleControl
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.InternalRichTextBox = new System.Windows.Forms.RichTextBox();
      this.SuspendLayout();
      // 
      // richTextBoxConsole
      // 
      this.InternalRichTextBox.AcceptsTab = true;
      this.InternalRichTextBox.BackColor = System.Drawing.Color.Black;
      this.InternalRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.InternalRichTextBox.Font = new System.Drawing.Font("Consolas", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.InternalRichTextBox.ForeColor = System.Drawing.Color.White;
      this.InternalRichTextBox.Location = new System.Drawing.Point(0, 0);
      this.InternalRichTextBox.Name = "InternalRichTextBox";
      this.InternalRichTextBox.ReadOnly = true;
      this.InternalRichTextBox.Size = new System.Drawing.Size(150, 150);
      this.InternalRichTextBox.TabIndex = 0;
      this.InternalRichTextBox.Text = "";
      // 
      // ConsoleControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.InternalRichTextBox);
      this.Name = "ConsoleControl";
      this.ResumeLayout(false);

    }

    #endregion
  }
}
