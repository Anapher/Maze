using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;

namespace MazeTemplates.Wizard.Administration
{
    public class AdministrationModuleWizard : IWizard
    {
        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind,
            object[] customParams)
        {
            var projectName = replacementsDictionary["$safeprojectname$"];
            var match = Regex.Match(projectName, "^([A-Za-z0-9\\.]+)\\.Administration$");
            if (!match.Success)
            {
                MessageBox.Show(
                    $"Your project name \"{projectName}\" is not a valid name for a Maze administration module. It must have the scheme YourName.Administration.");
                replacementsDictionary.Add("ModuleNamePlaceholder", "INVALID_NAME");
                return;
            }

            replacementsDictionary.Add("ModuleNamePlaceholder", match.Groups[1].Value);

            var form = new AdministrationModuleForm();
            form.ShowDialog();
            if (form.Parameters == null)
                return;

            foreach (var formParameter in form.Parameters)
                replacementsDictionary.Add(formParameter.Key, formParameter.Value);
        }

        public void ProjectFinishedGenerating(Project project)
        {
        }

        public void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
        }

        public bool ShouldAddProjectItem(string filePath) => true;

        public void BeforeOpeningFile(ProjectItem projectItem)
        {
        }

        public void RunFinished()
        {
        }
    }
}