using System;
using System.IO;
using FileExplorer.Shared.Dtos;

// ReSharper disable once CheckNamespace
namespace FileExplorer.Administration.Models.Args
{
    public class EntryUpdatedEventArgs : EventArgs
    {
        public EntryUpdatedEventArgs(FileExplorerEntry entry, string oldPath)
        {
            Entry = entry;
            OldPath = oldPath;
            OldParentPath = Path.GetDirectoryName(oldPath);
        }

        public FileExplorerEntry Entry { get; }
        public string OldPath { get; }
        public string OldParentPath { get; }
    }
}