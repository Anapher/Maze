using System.ComponentModel.DataAnnotations;
using Tasks.Common.Server.Filters;
using Tasks.Infrastructure.Administration.Library;
using Tasks.Infrastructure.Administration.Library.Filter;
using Tasks.Infrastructure.Administration.PropertyGrid;
using Tasks.Infrastructure.Administration.PropertyGrid.Attributes;
using Tasks.Infrastructure.Core;
using Unclassified.TxLib;

namespace Tasks.Common.Administration.Filters
{
    public class OperatingSystemViewModel : PropertyGridViewModel, IFilterViewModel<OperatingSystemFilterInfo>
    {
        public OperatingSystemViewModel()
        {
            this.RegisterProperty(() => Windows7, "Windows 7",
                Tx.T("TasksCommon:Filters.OperatingSystem.Properties.Description", "system", "Windows 7"), "Desktop");
            this.RegisterProperty(() => Windows81, "Windows 8.1",
                Tx.T("TasksCommon:Filters.OperatingSystem.Properties.Description", "system", "Windows 8.1"), "Desktop");
            this.RegisterProperty(() => Windows10, "Windows 10",
                Tx.T("TasksCommon:Filters.OperatingSystem.Properties.Description", "system", "Windows 10"), "Desktop");
            this.RegisterProperty(() => WindowsServer2008, "Windows Server 2008",
                Tx.T("TasksCommon:Filters.OperatingSystem.Properties.Description", "system", "Windows Server 2008"), "Server");
            this.RegisterProperty(() => WindowsServer2012, "Windows Server 2012",
                Tx.T("TasksCommon:Filters.OperatingSystem.Properties.Description", "system", "Windows Server 2012"), "Server");
            this.RegisterProperty(() => WindowsServer2016, "Windows Server 2016",
                Tx.T("TasksCommon:Filters.OperatingSystem.Properties.Description", "system", "Windows Server 2016"), "Server");
        }

        [CheckBoxBoolean]
        public bool Windows7 { get; set; }

        [CheckBoxBoolean]
        public bool Windows81 { get; set; }

        [CheckBoxBoolean]
        public bool Windows10 { get; set; }

        [CheckBoxBoolean]
        public bool WindowsServer2008 { get; set; }

        [CheckBoxBoolean]
        public bool WindowsServer2012 { get; set; }

        [CheckBoxBoolean]
        public bool WindowsServer2016 { get; set; }

        public void Initialize(OperatingSystemFilterInfo model)
        {
            Windows7 = model.Windows7;
            Windows81 = model.Windows81;
            Windows10 = model.Windows10;
            WindowsServer2008 = model.WindowsServer2008;
            WindowsServer2012 = model.WindowsServer2012;
            WindowsServer2016 = model.WindowsServer2016;

            OnPropertiesChanged();
        }

        public ValidationResult ValidateInput()
        {
            if (Windows7 || Windows81 || Windows10 || WindowsServer2008 || WindowsServer2012 || WindowsServer2016)
                return ValidationResult.Success;

            return new ValidationResult(Tx.T("TasksCommon:Filters.OperatingSystem.Errors.NoSystemSelected"));
        }

        public ValidationResult ValidateContext(MazeTask mazeTask) => ValidationResult.Success;

        public OperatingSystemFilterInfo Build() =>
            new OperatingSystemFilterInfo
            {
                Windows7 = Windows7,
                Windows81 = Windows81,
                Windows10 = Windows10,
                WindowsServer2008 = WindowsServer2008,
                WindowsServer2012 = WindowsServer2012,
                WindowsServer2016 = WindowsServer2016
            };
    }
}