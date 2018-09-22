using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SystemInformation.Shared.Dtos;
using SystemInformation.Shared.Dtos.Value;
using Unclassified.TxLib;

namespace SystemInformation.Administration.ViewModels
{
    public class SystemInfoViewModel
    {
        public SystemInfoViewModel(SystemInfoDto systemInfoDto)
        {
            Value = systemInfoDto.Value;

            if (systemInfoDto.Name[0] == '@')
                Name = Tx.T($"SystemInformation:Labels.{systemInfoDto.Name.TrimStart('@')}");
            else
                Name = systemInfoDto.Name;

            Childs = systemInfoDto.Childs?.Select(x => new SystemInfoViewModel(x)).OrderBy(x => x.Childs.Any()).ThenBy(x => x.Name).ToList() ??
                     new List<SystemInfoViewModel>(0);
        }

        public string Name { get; }
        public ValueDto Value { get; }
        public List<SystemInfoViewModel> Childs { get; }
        public bool HasChilds => Childs.Any();

        public string GetValueString()
        {
            switch (Value)
            {
                case var value when value is TextValueDto val:
                    return val.Value;
                case var value when value is TranslatedTextValueDto val:
                    return Tx.T($"SystemInformation:Labels.{val.TranslationKey}");
                case var value when value is NumberValueDto val:
                    return Tx.Number(val.Value);
                case var value when value is DataSizeValueDto val:
                    return Tx.DataSize(val.Value);
                case var value when value is ProgressValueDto val:
                    return $"{val.Value} / {val.Maximum}";
                case var value when value is CultureValueDto val:
                    var culture = CultureInfo.GetCultureInfo(val.Value);
                    return $"{culture.DisplayName} ({culture.TwoLetterISOLanguageName})";
                case var value when value is DateTimeValueDto val:
                    return Tx.Time(val.Value.LocalDateTime, TxTime.YearMonthLong | TxTime.HourMinuteSecond);
                case var value when value is BoolValueDto val:
                    return val.ToString();
                case var value when value is HeaderValueDto val:
                    return string.Empty;
                default:
                    return string.Empty;
            }
        }
    }
}