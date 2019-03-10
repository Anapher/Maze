using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MazeTemplates.Wizard.Administration
{
    public partial class AdministrationModuleForm : Form
    {
        public AdministrationModuleForm()
        {
            InitializeComponent();
        }

        public Dictionary<string, string> Parameters { get; set; }

        private void createButton_Click(object sender, EventArgs e)
        {
            string category;
            if (funRadio.Checked)
                category = "Fun";
            else if (interactionRadio.Checked)
                category = "Interaction";
            else if (systemRadio.Checked)
                category = "System";
            else
                category = "Surveillance";

            Parameters = new Dictionary<string, string>
            {
                {"$moduledescription$", descriptionTextBox.Text}, {"CommandCategory.System", $"CommandCategory.{category}"}
            };
            Close();
        }
    }
}
