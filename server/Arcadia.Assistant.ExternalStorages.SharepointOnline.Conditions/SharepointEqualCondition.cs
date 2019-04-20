namespace Arcadia.Assistant.ExternalStorages.SharepointOnline.Conditions
{
    using System;
    using System.Linq.Expressions;

    using Arcadia.Assistant.ExternalStorages.Abstractions;

    public class SharepointEqualCondition : BaseSharepointCondition
    {
        public SharepointEqualCondition(Expression<Func<StorageItem, object>> property, object value)
            : base(property, value)
        {
        }
    }
}