using System;
using System.Collections.Immutable;
using FileExplorer.Shared.Dtos;

// ReSharper disable once CheckNamespace
namespace FileExplorer.Administration.Models
{
    public class DirectoryEntriesUpdatedEventArgs : EventArgs
    {
        public DirectoryEntriesUpdatedEventArgs(string directoryPath, IImmutableList<FileExplorerEntry> entries,
            bool directoriesOnly)
        {
            DirectoryPath = directoryPath;
            Entries = entries;
            DirectoriesOnly = directoriesOnly;
        }

        public string DirectoryPath { get; }
        public IImmutableList<FileExplorerEntry> Entries { get; }
        public bool DirectoriesOnly { get; }
    }
}