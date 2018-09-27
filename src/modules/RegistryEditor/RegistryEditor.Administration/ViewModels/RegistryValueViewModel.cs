using System;
using RegistryEditor.Shared.Dtos;

namespace RegistryEditor.Administration.ViewModels
{
    public class RegistryValueViewModel
    {
        public RegistryValueViewModel(RegistryValueDto dto)
        {
            Dto = dto;

            switch (dto)
            {
                case var val when val is StringRegistryValueDto typedVal:
                    ValueString = typedVal.Value;
                    break;
                case var val when val is BinaryRegistryValueDto typedVal:
                    ValueString = BitConverter.ToString(typedVal.Value).Replace('-', ' ');
                    break;
                case var val when val is DWordRegistryValueDto typedVal:
                    ValueString = typedVal.Value.ToString();
                    break;
                case var val when val is QWordRegistryValueDto typedVal:
                    ValueString = typedVal.Value.ToString();
                    break;
                case var val when val is MultiStringRegistryValueDto typedVal:
                    ValueString = string.Join("\t", typedVal.Value);
                    break;
                case var val when val is ExpandableStringRegistryValueDto typedVal:
                    ValueString = typedVal.Value;
                    break;
            }
        }

        public RegistryValueDto Dto { get; }
        public string ValueString { get; }
    }
}