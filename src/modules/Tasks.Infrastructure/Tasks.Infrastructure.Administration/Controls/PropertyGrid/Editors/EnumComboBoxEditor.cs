using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;

namespace Tasks.Infrastructure.Administration.Controls.PropertyGrid.Editors
{
    public class EnumComboBoxEditor : ComboBoxEditorBase
    {
        public override int Priority { get; } = 0;

        public override bool IsSupported(PropertyItem propertyItem, Type propertyType)
        {
            return propertyType.IsEnum;
        }

        protected override IEnumerable CreateItemsSource(PropertyEditorContext<ComboBox> context)
        {
            var enumValues = Enum.GetValues(context.Property.PropertyType).Cast<object>().Distinct();

            return enumValues.Select(enumValue =>
                new EnumerationMember {Value = enumValue, Description = GetDescription(enumValue, context.Property.PropertyType)}).ToArray();
        }

        protected override void InitializeControl(PropertyEditorContext<ComboBox> context)
        {
            base.InitializeControl(context);

            context.Editor.SelectedValuePath = "Value";
            context.Editor.DisplayMemberPath = "Description";
        }

        private static string GetDescription(object enumValue, Type enumType)
        {
            return enumType.GetField(enumValue.ToString()).GetCustomAttribute<DescriptionAttribute>()?.Description ?? enumValue.ToString();
        }

        public class EnumerationMember
        {
            public string Description { get; set; }
            public object Value { get; set; }
        }
    }
}