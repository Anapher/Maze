using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Orcus.Administration.Library.Views
{
    public abstract class ResourceDictionaryProvider
    {
        protected readonly ResourceDictionary ResourceDictionary;

        protected ResourceDictionaryProvider(Uri dictionaryUri)
        {
            ResourceDictionary = new ResourceDictionary {Source = dictionaryUri};
        }

        protected T GetElement<T>([CallerMemberName] string key = null) => (T) ResourceDictionary[key];
        protected Viewbox GetIcon([CallerMemberName] string key = null) => (Viewbox)ResourceDictionary[key + "Icon"];
    }
}