namespace Arcadia.Assistant.ExternalStorages.SharepointOnline.Conditions
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    using Arcadia.Assistant.ExternalStorages.Abstractions;

    public abstract class BaseSharepointCondition : ICondition
    {
        protected BaseSharepointCondition(Expression<Func<StorageItem, object>> property, object value)
        {
            // ToDo: Fix this
            var propertyInfo = ((MemberExpression)property.Body).Member;

            if (propertyInfo.MemberType != MemberTypes.Property)
            {
                throw new ArgumentException("Should be a property expression", nameof(property));
            }

            this.Property = property;
            this.Value = value;
        }

        public Expression<Func<StorageItem, object>> Property { get; }

        public object Value { get; }
    }
}