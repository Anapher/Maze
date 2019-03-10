using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using EnvDTE;
using MazeTemplates.Wizard.Administration;
using Microsoft.VisualStudio.TemplateWizard;

namespace MazeTemplates.Wizard.Client
{
    public class ClientModuleWizard : IWizard
    {
        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind,
            object[] customParams)
        {
            var projectName = replacementsDictionary["$safeprojectname$"];
            var match = Regex.Match(projectName, "^([A-Za-z0-9\\.]+)\\.Client$");
            if (!match.Success)
            {
                MessageBox.Show(
                    $"Your project name \"{projectName}\" is not a valid name for a Maze administration module. It must have the scheme YourName.Client.");
                replacementsDictionary.Add("ModuleNamePlaceholder", "INVALID_NAME");
                return;
            }

            replacementsDictionary.Add("ModuleNamePlaceholder", match.Groups[1].Value);
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