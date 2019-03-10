namespace MazeTemplates.Wizard.Administration
{
    partial class AdministrationModuleForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.funRadio = new System.Windows.Forms.RadioButton();
            this.interactionRadio = new System.Windows.Forms.RadioButton();
            this.systemRadio = new System.Windows.Forms.RadioButton();
            this.surveillanceRadio = new System.Windows.Forms.RadioButton();
            this.descriptionTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.createButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Command Category";
            // 
            // funRadio
            // 
            this.funRadio.AutoSize = true;
            this.funRadio.Location = new System.Drawing.Point(12, 25);
            this.funRadio.Name = "funRadio";
            this.funRadio.Size = new System.Drawing.Size(43, 17);
            this.funRadio.TabIndex = 1;
            this.funRadio.Text = "Fun";
            this.funRadio.UseVisualStyleBackColor = true;
            // 
            // interactionRadio
            // 
            this.interactionRadio.AutoSize = true;
            this.interactionRadio.Location = new System.Drawing.Point(61, 25);
            this.interactionRadio.Name = "interactionRadio";
            this.interactionRadio.Size = new System.Drawing.Size(75, 17);
            this.interactionRadio.TabIndex = 2;
            this.interactionRadio.Text = "Interaction";
            this.interactionRadio.UseVisualStyleBackColor = true;
            // 
            // systemRadio
            // 
            this.systemRadio.AutoSize = true;
            this.systemRadio.Checked = true;
            this.systemRadio.Location = new System.Drawing.Point(142, 25);
            this.systemRadio.Name = "systemRadio";
            this.systemRadio.Size = new System.Drawing.Size(59, 17);
            this.systemRadio.TabIndex = 3;
            this.systemRadio.TabStop = true;
            this.systemRadio.Text = "System";
            this.systemRadio.UseVisualStyleBackColor = true;
            // 
            // surveillanceRadio
            // 
            this.surveillanceRadio.AutoSize = true;
            this.surveillanceRadio.Location = new System.Drawing.Point(207, 25);
            this.surveillanceRadio.Name = "surveillanceRadio";
            this.surveillanceRadio.Size = new System.Drawing.Size(83, 17);
            this.surveillanceRadio.TabIndex = 4;
            this.surveillanceRadio.Text = "Surveillance";
            this.surveillanceRadio.UseVisualStyleBackColor = true;
            // 
            // descriptionTextBox
            // 
            this.descriptionTextBox.Location = new System.Drawing.Point(15, 84);
            this.descriptionTextBox.Name = "descriptionTextBox";
            this.descriptionTextBox.Size = new System.Drawing.Size(569, 20);
            this.descriptionTextBox.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Description";
            // 
            // createButton
            // 
            this.createButton.Location = new System.Drawing.Point(465, 124);
            this.createButton.Name = "createButton";
            this.createButton.Size = new System.Drawing.Size(119, 23);
            this.createButton.TabIndex = 7;
            this.createButton.Text = "Create";
            this.createButton.UseVisualStyleBackColor = true;
            this.createButton.Click += new System.EventHandler(this.createButton_Click);
            // 
            // AdministrationModuleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(596, 159);
            this.Controls.Add(this.createButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.descriptionTextBox);
            this.Controls.Add(this.surveillanceRadio);
            this.Controls.Add(this.systemRadio);
            this.Controls.Add(this.interactionRadio);
            this.Controls.Add(this.funRadio);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AdministrationModuleForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Create Administration Module";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton funRadio;
        private System.Windows.Forms.RadioButton interactionRadio;
        private System.Windows.Forms.RadioButton systemRadio;
        private System.Windows.Forms.RadioButton surveillanceRadio;
        private System.Windows.Forms.TextBox descriptionTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button createButton;
    }
}