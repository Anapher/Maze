using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using FileExplorer.Administration.Utilities;
using FileExplorer.Administration.ViewModels.Explorer;
using FileExplorer.Administration.ViewModels.Explorer.Base;
using FileExplorer.Shared.Dtos;
using Prism.Commands;
using Prism.Mvvm;
using Unclassified.TxLib;

namespace FileExplorer.Administration.ViewModels
{
    public class PropertiesViewModel : BindableBase
    {
        public PropertiesViewModel(FileViewModel fileViewModel, FilePropertiesDto dto)
        {
            var properties = GeneralPropertyViewModel.CreateFileProperties(fileViewModel, dto).ToList();
            GeneralProperties = CreateGeneralProperties(properties);
            Entry = fileViewModel;
            DetailsViewModel = new DetailsPropertyViewModel(dto.Properties);
        }

        public EntryViewModel Entry { get; }
        public bool IsFile => !Entry.IsDirectory;
        public ListCollectionView GeneralProperties { get; }
        public DetailsPropertyViewModel DetailsViewModel { get; }

        private ListCollectionView CreateGeneralProperties(IList properties)
        {
            var result = new ListCollectionView(properties);
            result.GroupDescriptions.Add(new PropertyGroupDescription(nameof(GeneralPropertyViewModel.Group)));

            return result;
        }
    }

    public class DetailsPropertyViewModel
    {
        private readonly List<FileProperty> _properties;
        private DelegateCommand _copyAllCommand;
        private DelegateCommand<IList> _copyCommand;
        private DelegateCommand<IList> _copyNameCommand;
        private DelegateCommand<IList> _copyValueCommand;

        public DetailsPropertyViewModel(IEnumerable<FileProperty> properties)
        {
            _properties = TranslateProperties(properties).ToList();

            View = new ListCollectionView(_properties);
            View.GroupDescriptions.Add(new PropertyGroupDescription(nameof(FileProperty.Group)));
            View.SortDescriptions.Add(new SortDescription(nameof(FileProperty.Name), ListSortDirection.Ascending));
        }

        public ListCollectionView View { get; }

        public DelegateCommand<IList> CopyCommand
        {
            get
            {
                return _copyCommand ?? (_copyCommand = new DelegateCommand<IList>(parameter =>
                {
                    var stringBuilder = new StringBuilder();
                    foreach (var fileProperty in parameter.Cast<FileProperty>())
                        stringBuilder.AppendFormat("{0} = {1}\r\n", fileProperty.Name, fileProperty.Value);

                    stringBuilder.Length -= 2;
                    Clipboard.SetDataObject(stringBuilder.ToString());
                }));
            }
        }

        public DelegateCommand<IList> CopyNameCommand
        {
            get
            {
                return _copyNameCommand ?? (_copyNameCommand = new DelegateCommand<IList>(parameter =>
                {
                    var stringBuilder = new StringBuilder();
                    foreach (var fileProperty in parameter.Cast<FileProperty>())
                        stringBuilder.AppendLine(fileProperty.Name);

                    stringBuilder.Length -= 2;
                    Clipboard.SetDataObject(stringBuilder.ToString());
                }));
            }
        }

        public DelegateCommand<IList> CopyValueCommand
        {
            get
            {
                return _copyValueCommand ?? (_copyValueCommand = new DelegateCommand<IList>(parameter =>
                {
                    var stringBuilder = new StringBuilder();
                    foreach (var fileProperty in parameter.Cast<FileProperty>())
                        stringBuilder.AppendLine(fileProperty.Value);

                    stringBuilder.Length -= 2;
                    Clipboard.SetDataObject(stringBuilder.ToString());
                }));
            }
        }

        public DelegateCommand CopyAllCommand
        {
            get
            {
                return _copyAllCommand ?? (_copyAllCommand = new DelegateCommand(() =>
                {
                    Clipboard.SetDataObject(string.Join(Environment.NewLine,
                        _properties.Select(x => $"{x.Name} = {x.Value}")));
                }));
            }
        }

        private static IEnumerable<FileProperty> TranslateProperties(IEnumerable<FileProperty> properties)
        {
            foreach (var fileProperty in properties)
            {
                string translatedName;
                if (fileProperty.FormatId != null && fileProperty.PropertyId != null)
                    translatedName = fileProperty.GetShellDisplayName();
                else
                    Tx.TryGetText($"FileExplorer:Properties.Details.Translations.{fileProperty.Name}",
                        out translatedName);

                if (string.IsNullOrEmpty(translatedName))
                    translatedName = fileProperty.Name;

                string value;
                if (fileProperty.ValueType == FilePropertyValueType.DateTime)
                    value = DateTimeOffset.ParseExact(fileProperty.Value, "O", CultureInfo.InvariantCulture)
                        .LocalDateTime.ToString("F");
                else
                    value = fileProperty.Value;

                yield return new FileProperty {Group = fileProperty.Group, Name = translatedName, Value = value};
            }
        }
    }

    public class GeneralPropertyViewModel
    {
        public int Group { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }

        public static IEnumerable<GeneralPropertyViewModel> CreateFileProperties(FileViewModel fileViewModel,
            FilePropertiesDto dto)
        {
            yield return new GeneralPropertyViewModel
            {
                Label = Tx.TC("FileExplorer:Type"),
                Value = fileViewModel.Description
            };
            if (!string.IsNullOrEmpty(dto.OpenWithProgramName))
                yield return new GeneralPropertyViewModel
                {
                    Label = Tx.TC("FileExplorer:Properties.General.OpenWith"),
                    Value = dto.OpenWithProgramName + "\r\n" + dto.OpenWithProgramPath
                };
            yield return new GeneralPropertyViewModel
            {
                Group = 2,
                Label = Tx.TC("FileExplorer:Properties.General.Location"),
                Value = fileViewModel.Source.Path
            };
            yield return new GeneralPropertyViewModel
            {
                Group = 2,
                Label = Tx.TC("FileExplorer:Size"),
                Value = Tx.DataSize(dto.Size) + $" ({Tx.Number(dto.SizeOnDisk)} B)"
            };
            yield return new GeneralPropertyViewModel
            {
                Group = 2,
                Label = Tx.TC("FileExplorer:Properties.General.SizeOnDisk"),
                Value = Tx.DataSize(dto.SizeOnDisk) + $" ({Tx.Number(dto.SizeOnDisk)} B)"
            };
            yield return new GeneralPropertyViewModel
            {
                Group = 3,
                Label = Tx.TC("FileExplorer:Properties.General.Created"),
                Value = dto.CreationTime.LocalDateTime.ToString("F", CultureInfo.CurrentUICulture)
            };
            yield return new GeneralPropertyViewModel
            {
                Group = 3,
                Label = Tx.TC("FileExplorer:Properties.General.Modified"),
                Value = dto.LastWriteTime.LocalDateTime.ToString("F", CultureInfo.CurrentUICulture)
            };
            yield return new GeneralPropertyViewModel
            {
                Group = 3,
                Label = Tx.TC("FileExplorer:Properties.General.Accessed"),
                Value = dto.LastAccessTime.LocalDateTime.ToString("F", CultureInfo.CurrentUICulture)
            };
            yield return new GeneralPropertyViewModel
            {
                Group = 4,
                Label = Tx.TC("FileExplorer:Properties.General.Attributes"),
                Value = dto.Attributes.ToString()
            };
        }
    }
}