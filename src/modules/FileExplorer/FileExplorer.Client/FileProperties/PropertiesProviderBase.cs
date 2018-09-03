using System;
using System.Collections;
using System.Linq;
using FileExplorer.Shared.Dtos;

namespace FileExplorer.Client.FileProperties
{
    public abstract class PropertiesProviderBase
    {
        protected static (string, FilePropertyValueType) ObjectToString(object obj)
        {
            if (obj is ICollection collection)
                return (string.Join(", ", collection.Cast<object>().Select(x => x?.ToString()).ToArray()),
                    FilePropertyValueType.String);
            if (obj is DateTime dateTime)
                return (dateTime.ToUniversalTime().ToString("O"), FilePropertyValueType.DateTime);

            return (obj?.ToString(), FilePropertyValueType.String);
        }
    }
}