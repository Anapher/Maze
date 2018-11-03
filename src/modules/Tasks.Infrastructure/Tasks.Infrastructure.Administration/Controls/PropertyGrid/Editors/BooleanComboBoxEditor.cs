using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Tasks.Infrastructure.Administration.Controls.PropertyGrid.Editors
{
    public class BooleanComboBoxEditor : ComboBoxEditorBase
    {
        public override int Priority { get; } = 0;

        public override bool IsSupported(PropertyItem propertyItem, Type propertyType)
        {
            if (propertyType == typeof(bool) || propertyType == typeof(bool?))
                return true;

            return false;
        }

        protected override IEnumerable CreateItemsSource(PropertyEditorContext<ComboBox> context)
        {
            var items = new List<BoolMember>
            {
                new BoolMember {Value = false, Description = bool.FalseString},
                new BoolMember {Value = true, Description = bool.TrueString}
            };
            if (context.Property.PropertyType == typeof(bool?))
                items.Add(new BoolMember {Value = null, Description = "Null"});

            return items;
        }

        protected override void InitializeControl(PropertyEditorContext<ComboBox> context)
        {
            base.InitializeControl(context);

            context.Editor.SelectedValuePath = "Value";
            context.Editor.DisplayMemberPath = "Description";
        }

        public class BoolMember
        {
            public string Description { get; set; }
            public bool? Value { get; set; }
        }
    }
}