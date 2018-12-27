using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Orcus.Administration.Library.Views
{
    /// <summary>
    ///     A base class for creating bindings to a XAML dictionary
    /// </summary>
    public abstract class ResourceDictionaryProvider
    {
        protected readonly ResourceDictionary ResourceDictionary;

        /// <summary>
        ///     Create a new instance of <see cref="ResourceDictionaryProvider"/> with the dictionary uri
        /// </summary>
        /// <param name="dictionaryUri">The uri of the dictionary</param>
        protected ResourceDictionaryProvider(Uri dictionaryUri)
        {
            ResourceDictionary = new ResourceDictionary {Source = dictionaryUri};
        }

        /// <summary>
        ///     Get an element from the resource dictionary
        /// </summary>
        /// <typeparam name="T">The type of the element</typeparam>
        /// <param name="key">The name of the element</param>
        /// <returns>Return the element from the dictionary</returns>
        protected T GetElement<T>([CallerMemberName] string key = null) => (T) ResourceDictionary[key];

        /// <summary>
        ///     Get an icon from the resource dictionary (of type <see cref="Viewbox"/>)
        /// </summary>
        /// <param name="key">The name of the icon. This name will be suffixed with "Icon".</param>
        /// <returns>Return the icon from the resource dictionary</returns>
        protected Viewbox GetIcon([CallerMemberName] string key = null) => (Viewbox)ResourceDictionary[key + "Icon"];
    }
}