using System;
using System.Windows;
using Xceed.Wpf.Toolkit;

namespace Tasks.Infrastructure.Administration.Controls.PropertyGrid.Editors
{
    public class TimeSpanEditor : PropertyEditor<TimeSpanUpDown>
    {
        protected override DependencyProperty GetDependencyProperty()
        {
            return TimeSpanUpDown.ValueProperty;
        }

        public override int Priority { get; } = 0;

        public override bool IsSupported(PropertyItem propertyItem, Type propertyType)
        {
            if (propertyType == typeof(TimeSpan) || propertyType == typeof(TimeSpan?))
                return true;

            return false;
        }

        protected override TimeSpanUpDown CreateEditor(PropertyEditorContext<TimeSpanUpDown> context)
        {
            return new PropertyGridEditorTimeSpanUpDown();
        }

        protected override void InitializeControl(PropertyEditorContext<TimeSpanUpDown> context)
        {
            base.InitializeControl(context);

            context.Editor.Minimum = TimeSpan.Zero;
        }
    }

    public class PropertyGridEditorTimeSpanUpDown : TimeSpanUpDown
    {
        static PropertyGridEditorTimeSpanUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorTimeSpanUpDown),
                new FrameworkPropertyMetadata(typeof(PropertyGridEditorTimeSpanUpDown)));
        }
    }
}