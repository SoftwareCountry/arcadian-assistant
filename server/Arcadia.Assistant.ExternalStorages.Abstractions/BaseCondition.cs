namespace Arcadia.Assistant.ExternalStorages.Abstractions
{
    using System;
    using System.Linq.Expressions;

    public abstract class BaseCondition : ICondition
    {
        protected BaseCondition(Expression<Func<StorageItem, object>> property, object value)
        {
            new PropertyNameParser().EnsureExpressionIsProperty(property);

            this.Property = property;
            this.Value = value;
        }

        public Expression<Func<StorageItem, object>> Property { get; }

        public object Value { get; }
    }
}