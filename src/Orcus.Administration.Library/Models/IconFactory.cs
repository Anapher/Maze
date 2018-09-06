using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Orcus.Administration.Library.Models
{
    public static class IconFactory
    {
        /// <summary>
        ///     Create a new icon from a single instance. Please note that this instance cannot be an UIElement as the icon must be freezable
        /// </summary>
        /// <param name="obj">The object instance</param>
        /// <returns>Return the icon factory that wraps the <see cref="obj"/></returns>
        public static IIconFactory FromInstance(object obj) => new ObjectFactory(obj);

        /// <summary>
        ///     Create a new icon from a factory method
        /// </summary>
        /// <typeparam name="T">The type of the icon factory</typeparam>
        /// <param name="factory">The factory that creates the icon</param>
        /// <returns>Return the icon factory that wraps the <see cref="factory"/></returns>
        public static IIconFactory FromFactory<T>(Func<T> factory) => new FunctionFactory<T>(factory);

        /// <summary>
        ///     Create a new icon from an image source
        /// </summary>
        /// <param name="imageSource">The image source that contains the icon</param>
        /// <returns>Return the icon factory that wraps the <see cref="imageSource"/></returns>
        public static IIconFactory FromImage(ImageSource imageSource) => new FunctionFactory<Image>(() => new Image{Source = imageSource});

        private class ObjectFactory : IIconFactory
        {
            private readonly object _obj;

            public ObjectFactory(object obj)
            {
                if (obj is UIElement)
                    throw new ArgumentException("UIElement cannot be used as a single instance icon as it doesn't support multiple visual parents");

                _obj = obj;
            }

            public object Create() => _obj;
        }

        private class FunctionFactory<T> : IIconFactory
        {
            private readonly Func<T> _objFactory;

            public FunctionFactory(Func<T> objFactory)
            {
                _objFactory = objFactory;
            }

            public object Create() => _objFactory();
        }
    }
}