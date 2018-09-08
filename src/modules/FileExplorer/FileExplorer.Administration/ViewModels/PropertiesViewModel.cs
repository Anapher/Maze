using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using FileExplorer.Administration.Rest;
using FileExplorer.Administration.Utilities;
using FileExplorer.Administration.ViewModels.Explorer;
using FileExplorer.Administration.ViewModels.Explorer.Base;
using FileExplorer.Shared.Channels;
using FileExplorer.Shared.Dtos;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Extensions;
using Prism.Commands;
using Prism.Mvvm;
using Unclassified.TxLib;

namespace FileExplorer.Administration.ViewModels
{
    public class PropertiesViewModel : BindableBase
    {
        public PropertiesViewModel(FileViewModel fileViewModel, FilePropertiesDto dto, IPackageRestClient restClient)
        {
            var properties = GeneralPropertyViewModel.CreateFileProperties(fileViewModel, dto).ToList();
            GeneralProperties = CreateGeneralProperties(properties);
            Entry = fileViewModel;
            DetailsViewModel = new DetailsPropertyViewModel(dto.Properties);

            if (!Entry.IsDirectory)
                HashViewModels = Enum.GetValues(typeof(FileHashAlgorithm)).Cast<FileHashAlgorithm>()
                    .Select(x => new ComputeHashViewModel(Entry.Source.Path, x, restClient)).ToList();
        }

        public EntryViewModel Entry { get; }
        public bool IsFile => !Entry.IsDirectory;
        public ListCollectionView GeneralProperties { get; }
        public DetailsPropertyViewModel DetailsViewModel { get; }
        public List<ComputeHashViewModel> HashViewModels { get; }

        private ListCollectionView CreateGeneralProperties(IList properties)
        {
            var result = new ListCollectionView(properties);
            result.GroupDescriptions.Add(new PropertyGroupDescription(nameof(GeneralPropertyViewModel.Group)));

            return result;
        }
    }

    public class ComputeHashViewModel : BindableBase
    {
        private readonly string _path;
        private readonly IPackageRestClient _restClient;

        private DelegateCommand _copyHashCommand;

        private string _hashValue;
        private bool _isComputing;

        private double _progress;

        private DelegateCommand _startComputingCommand;

        public ComputeHashViewModel(string path, FileHashAlgorithm hashAlgorithm, IPackageRestClient restClient)
        {
            _path = path;
            HashAlgorithm = hashAlgorithm;
            _restClient = restClient;
        }

        public FileHashAlgorithm HashAlgorithm { get; }

        public bool IsComputing
        {
            get => _isComputing;
            set => SetProperty(ref _isComputing, value);
        }

        public string HashValue
        {
            get => _hashValue;
            set => SetProperty(ref _hashValue, value);
        }

        public double Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        public DelegateCommand CopyHashCommand
        {
            get
            {
                return _copyHashCommand ?? (_copyHashCommand =
                           new DelegateCommand(() => { Clipboard.SetText(HashValue); }, () => HashValue != null).ObservesProperty(() => HashValue));
            }
        }

        public DelegateCommand StartComputingCommand
        {
            get
            {
                return _startComputingCommand ?? (_startComputingCommand = new DelegateCommand(async () =>
                {
                    IsComputing = true;
                    try
                    {
                        using (var channel = await FileExplorerResource.ComputeHash(_restClient))
                        {
                            channel.Interface.ProgressChanged += OnProgressChanged;
                            var value = await channel.Interface.ComputeAsync(_path, HashAlgorithm);
                            HashValue = BitConverter.ToString(value).Replace("-", null);
                        }
                    }
                    catch (Exception e)
                    {
                        e.ShowMessage(null);
                    }
                    finally
                    {
                        IsComputing = false;
                    }
                }, () => !IsComputing && HashValue == null).ObservesProperty(() => IsComputing).ObservesProperty(() => HashValue));
            }
        }

        private void OnProgressChanged(object sender, ProgressChangedArgs e)
        {
            Progress = e.Progress;
        }
    }

    public class HashesViewModel
    {
        private readonly IPackageRestClient _fileViewModel;

        public HashesViewModel(IPackageRestClient fileViewModel)
        {
            _fileViewModel = fileViewModel;
        }

        private async Task Test()
        {
            using (var channel = await FileExplorerResource.ComputeHash(_fileViewModel))
            {
                channel.Interface.ProgressChanged += InterfaceOnProgressChanged;
                await channel.Interface.ComputeAsync("", FileHashAlgorithm.MD5);
            }
        }

        private void InterfaceOnProgressChanged(object sender, ProgressChangedArgs e)
        {
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
                    Clipboard.SetDataObject(string.Join(Environment.NewLine, _properties.Select(x => $"{x.Name} = {x.Value}")));
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
                    Tx.TryGetText($"FileExplorer:Properties.Details.Translations.{fileProperty.Name}", out translatedName);

                if (string.IsNullOrEmpty(translatedName))
                    translatedName = fileProperty.Name;

                string value;
                if (fileProperty.ValueType == FilePropertyValueType.DateTime)
                    value = DateTimeOffset.ParseExact(fileProperty.Value, "O", CultureInfo.InvariantCulture).LocalDateTime.ToString("F");
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

        public static IEnumerable<GeneralPropertyViewModel> CreateFileProperties(FileViewModel fileViewModel, FilePropertiesDto dto)
        {
            yield return new GeneralPropertyViewModel {Label = Tx.TC("FileExplorer:Type"), Value = fileViewModel.Description};
            if (!string.IsNullOrEmpty(dto.OpenWithProgramName))
                yield return new GeneralPropertyViewModel
                {
                    Label = Tx.TC("FileExplorer:Properties.General.OpenWith"), Value = dto.OpenWithProgramName + "\r\n" + dto.OpenWithProgramPath
                };
            yield return new GeneralPropertyViewModel
            {
                Group = 2, Label = Tx.TC("FileExplorer:Properties.General.Location"), Value = fileViewModel.Source.Path
            };
            yield return new GeneralPropertyViewModel
            {
                Group = 2, Label = Tx.TC("FileExplorer:Size"), Value = Tx.DataSize(dto.Size) + $" ({Tx.Number(dto.SizeOnDisk)} B)"
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
                Group = 4, Label = Tx.TC("FileExplorer:Properties.General.Attributes"), Value = dto.Attributes.ToString()
            };
        }
    }
}