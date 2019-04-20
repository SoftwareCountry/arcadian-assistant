namespace Arcadia.Assistant.ExternalStorages.SharepointOnline.Conditions
{
    using System;
    using System.Linq.Expressions;

    using Arcadia.Assistant.ExternalStorages.Abstractions;
    using Arcadia.Assistant.ExternalStorages.SharepointOnline.Helpers;

    public abstract class BaseSharepointCondition : ICondition
    {
        protected BaseSharepointCondition(Expression<Func<StorageItem, object>> property, object value)
        {
            new PropertyNameParser().EnsureExpressionIsProperty(property);

            this.Property = property;
            this.Value = value;
        }

        public Expression<Func<StorageItem, object>> Property { get; }

        public object Value { get; }
    }
}