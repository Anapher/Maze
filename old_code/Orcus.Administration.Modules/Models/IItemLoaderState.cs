using System.Collections.Generic;

namespace Orcus.Administration.Modules.Models
{
    public interface IItemLoaderState
    {
        LoadingStatus LoadingStatus { get; }
        int ItemsCount { get; }
        IDictionary<string, LoadingStatus> SourceLoadingStatus { get; }
    }
}