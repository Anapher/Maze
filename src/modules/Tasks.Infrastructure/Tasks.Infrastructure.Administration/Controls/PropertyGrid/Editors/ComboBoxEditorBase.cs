using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Tasks.Infrastructure.Administration.Controls.PropertyGrid.Editors
{
    public abstract class ComboBoxEditorBase : PropertyEditor<ComboBox>
    {
        protected override DependencyProperty GetDependencyProperty() => Selector.SelectedValueProperty;

        protected override ComboBox CreateEditor(PropertyEditorContext<ComboBox> context) => new PropertyGridEditorComboBox();

        protected abstract IEnumerable CreateItemsSource(PropertyEditorContext<ComboBox> context);

        protected override void InitializeControl(PropertyEditorContext<ComboBox> context)
        {
            base.InitializeControl(context);
            context.Editor.ItemsSource = CreateItemsSource(context);
        }
    }

    public class PropertyGridEditorComboBox : ComboBox
    {
        static PropertyGridEditorComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorComboBox),
                new FrameworkPropertyMetadata(typeof(PropertyGridEditorComboBox)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("PopupBorder") is Border popupBorder)
                popupBorder.BorderThickness = new Thickness(1, 1, 1, 1);
        }
    }
}