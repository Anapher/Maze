using Prism.Mvvm;
using Unclassified.TxLib;

namespace DeviceManager.Administration.ViewModels
{
    public class PropertiesViewModel : BindableBase
    {
        public DeviceViewModel Device { get; private set; }
        public string Title { get; private set; }

        public void Initialize(DeviceViewModel device)
        {
            Device = device;
            Title = $"{device.Caption} - {Tx.T("DeviceManager:Properties")}";
        }
    }
}