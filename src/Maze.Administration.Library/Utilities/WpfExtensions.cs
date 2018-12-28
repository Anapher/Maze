using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Maze.Administration.Library.Utilities
{
    /// <summary>
    ///     Wpf Extensions
    /// </summary>
    public static class WpfExtensions
    {
        /// <summary>
        ///     Searches for a visual parent
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="child">The child of the parent</param>
        /// <returns>The parent of the child</returns>
        public static T GetVisualParent<T>(this DependencyObject child) where T : Visual
        {
            while ((child != null) && !(child is T))
                child = VisualTreeHelper.GetParent(child);

            return (T) child;
        }

        /// <summary>
        ///     Go the visual tree upward to search for an <see cref="DependencyObject"/> of a given type
        /// </summary>
        /// <typeparam name="T">The type of the control that should be searched for</typeparam>
        /// <param name="source">The source <see cref="DependencyObject"/> that acts as the origin the search starts from.s</param>
        /// <returns>Return the found control or <code>null</code></returns>
        public static T VisualUpwardSearch<T>(DependencyObject source) where T : DependencyObject
        {
            DependencyObject returnVal = source;

            while (returnVal != null && !(returnVal is T))
            {
                DependencyObject tempReturnVal = null;
                if (returnVal is Visual || returnVal is Visual3D)
                {
                    tempReturnVal = VisualTreeHelper.GetParent(returnVal);
                }
                if (tempReturnVal == null)
                {
                    returnVal = LogicalTreeHelper.GetParent(returnVal);
                }
                else returnVal = tempReturnVal;
            }

            return returnVal as T;
        }

        /// <summary>
        ///     Finds a Child of a given item in the visual tree.
        /// </summary>
        /// <param name="parent">A direct parent of the queried item.</param>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="childName">x:Name or Name of child. </param>
        /// <returns>
        ///     The first parent item that matches the submitted type parameter.
        ///     If not matching item can be found,
        ///     a null parent is being returned.
        /// </returns>
        public static T FindChild<T>(DependencyObject parent, string childName)
            where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!String.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T) child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T) child;
                    break;
                }
            }

            return foundChild;
        }

        /// <summary>
        ///     Go the visual tree downward to search for an <see cref="DependencyObject"/> of a given type
        /// </summary>
        /// <typeparam name="T">The type of the control that should be searched for</typeparam>
        /// <param name="source">The source <see cref="DependencyObject"/> that acts as the origin the search starts from.s</param>
        /// <returns>Return the found control or <code>null</code></returns>
        public static T VisualDownwardSearch<T>(DependencyObject dependencyObject) where T : DependencyObject
        {
            if (dependencyObject == null)
            {
                return null;
            }

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
            {
                var child = VisualTreeHelper.GetChild(dependencyObject, i);

                var result = child as T ?? VisualDownwardSearch<T>(child);

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public static T GetDescendantByType<T>(Visual element) where T : Visual
        {
            if (element == null)
                return default(T);

            if (element.GetType() == typeof(T))
                return (T) element;

            T foundElement = null;
            (element as FrameworkElement)?.ApplyTemplate();

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);
                var visual = child as Visual;
                foundElement = GetDescendantByType<T>(visual);
                if (foundElement != null)
                    break;
            }
            return foundElement;
        }
    }
}