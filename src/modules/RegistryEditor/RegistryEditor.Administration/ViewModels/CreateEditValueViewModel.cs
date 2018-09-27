using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Prism.Commands;
using Prism.Mvvm;
using RegistryEditor.Shared.Dtos;
using Unclassified.TxLib;

namespace RegistryEditor.Administration.ViewModels
{
    public class CreateEditValueViewModel : BindableBase
    {
        private DelegateCommand _cancelCommand;
        private bool? _dialogResult;
        private string _name;
        private DelegateCommand _okCommand;
        private RegistryValueDto _value;

        public CreateEditValueViewModel(RegistryValueDto registryValue)
        {
            Value = JsonConvert.DeserializeObject<RegistryValueDto>(JsonConvert.SerializeObject(registryValue,
                new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()})); //clone
            Name = string.IsNullOrEmpty(registryValue.Name) ? Tx.T("RegistryEditor:DefaultValue") : registryValue.Name;
            Title = Tx.T("RegistryEditor:EditValueTitle", "name", Tx.T($"RegistryEditor:Type.{registryValue.Type}Value"));
        }

        public CreateEditValueViewModel(RegistryValueType type)
        {
            switch (type)
            {
                case RegistryValueType.String:
                    Value = new StringRegistryValueDto {Value = string.Empty};
                    break;
                case RegistryValueType.Binary:
                    Value = new BinaryRegistryValueDto {Value = new byte[0]};
                    break;
                case RegistryValueType.DWord:
                    Value = new DWordRegistryValueDto {Value = 0};
                    break;
                case RegistryValueType.QWord:
                    Value = new QWordRegistryValueDto {Value = 0};
                    break;
                case RegistryValueType.MultiString:
                    Value = new MultiStringRegistryValueDto {Value = new string[0]};
                    break;
                case RegistryValueType.ExpandableString:
                    Value = new ExpandableStringRegistryValueDto {Value = string.Empty};
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            Title = Tx.T("RegistryEditor:CreateValueTitle", "name", Tx.T($"RegistryEditor:Type.{type}Value"));
            IsCreate = true;
        }

        public bool? DialogResult
        {
            get => _dialogResult;
            set => SetProperty(ref _dialogResult, value);
        }

        public string Title { get; }
        public bool IsCreate { get; }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public RegistryValueDto Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        public DelegateCommand OkCommand
        {
            get
            {
                return _okCommand ?? (_okCommand = new DelegateCommand(() =>
                {
                    if (IsCreate)
                        Value.Name = Name;

                    DialogResult = true;
                }));
            }
        }

        public DelegateCommand CancelCommand
        {
            get { return _cancelCommand ?? (_cancelCommand = new DelegateCommand(() => { DialogResult = false; })); }
        }
    }
}