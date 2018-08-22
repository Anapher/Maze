using System.Windows;
using System.Windows.Controls;

namespace FileExplorer.Administration.Controls
{
    public class OneItemPanel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            for (int i = 0; i < InternalChildren.Count; i++)
            {
                UIElement element = InternalChildren[i];
                if (element.Visibility == Visibility.Visible)
                {
                    element.Measure(availableSize);
                    return element.DesiredSize;
                }
            }
            return new Size(0, 0);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            for (int i = 0; i < InternalChildren.Count; i++)
            {
                var element = InternalChildren[i];
                if (element.Visibility == Visibility.Visible)
                {
                    element.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
                    break;
                }
                element.Arrange(new Rect(0, 0, 0, 0));
            }

            return finalSize;
        }
    }
}