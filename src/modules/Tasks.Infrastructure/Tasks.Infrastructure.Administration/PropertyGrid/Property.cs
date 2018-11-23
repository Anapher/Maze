using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Tasks.Infrastructure.Administration.PropertyGrid
{
    /// <summary>
    ///     Generic implementation of <see cref="IProperty" />
    /// </summary>
    /// <typeparam name="T">The type of the property</typeparam>
    public class Property<T> : IProperty
    {
        private readonly IProvideEditableProperties _provideEditableProperties;
        private readonly PropertyInfo _propertyInfo;
        private readonly object _propertyProvidingObject;

        /// <summary>
        ///     Initialize <see cref="Property{T}" />
        /// </summary>
        /// <param name="provideEditableProperties">The object which has the property</param>
        /// <param name="property">The property</param>
        /// <param name="name">The dispaly name of the property</param>
        /// <param name="description">The description of the property</param>
        /// <param name="category">The category of the property</param>
        public Property(IProvideEditableProperties provideEditableProperties, Expression<Func<T>> property, string name,
            string description, string category)
        {
            _provideEditableProperties = provideEditableProperties;
            Name = name;
            Description = description;
            Category = category;

            var memberExpression = (MemberExpression) property.Body;
            _propertyInfo = (PropertyInfo) memberExpression.Member;

            var expr = memberExpression.Expression;
            var memberInfos = new Stack<MemberInfo>();

            // https://stackoverflow.com/a/3954131/4166138
            // "descend" toward's the root object reference:
            while (expr is MemberExpression)
            {
                var memberExpr = expr as MemberExpression;
                memberInfos.Push(memberExpr.Member);
                expr = memberExpr.Expression;
            }

            // fetch the root object reference:
            var constExpr = expr as ConstantExpression;
            var objReference = constExpr.Value;

            // "ascend" back whence we came from and resolve object references along the way:
            while (memberInfos.Count > 0)  // or some other break condition
            {
                var mi = memberInfos.Pop();
                if (mi.MemberType == MemberTypes.Property)
                {
                    objReference = objReference.GetType()
                        .GetProperty(mi.Name)
                        .GetValue(objReference, null);
                }
                else if (mi.MemberType == MemberTypes.Field)
                {
                    objReference = objReference.GetType()
                        .GetField(mi.Name)
                        .GetValue(objReference);
                }
            }

            _propertyProvidingObject = objReference;
        }

        /// <summary>
        ///     The current value of the property
        /// </summary>
        public T Value
        {
            get => (T) _propertyInfo.GetValue(_propertyProvidingObject, null);
            set => _propertyInfo.SetValue(_propertyProvidingObject, value, null);
        }

        /// <summary>
        ///     The display name of the property
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     The description of the property
        /// </summary>
        public string Description { get; }

        /// <summary>
        ///     The category of the property
        /// </summary>
        public string Category { get; }

        object IProperty.Value
        {
            get => Value;
            set => Value = (T) value;
        }

        PropertyInfo IProperty.PropertyInfo => _propertyInfo;
        Type IProperty.PropertyType => typeof (T);
        string IProperty.PropertyName => _propertyInfo.Name;
    }
}