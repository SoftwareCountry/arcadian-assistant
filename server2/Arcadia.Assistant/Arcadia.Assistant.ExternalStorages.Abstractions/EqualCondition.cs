namespace Arcadia.Assistant.ExternalStorages.Abstractions
{
    using System;
    using System.Linq.Expressions;

    public class EqualCondition : BaseCondition
    {
        public EqualCondition(Expression<Func<StorageItem, object>> property, object value)
            : base(property, value)
        {
        }
    }
}