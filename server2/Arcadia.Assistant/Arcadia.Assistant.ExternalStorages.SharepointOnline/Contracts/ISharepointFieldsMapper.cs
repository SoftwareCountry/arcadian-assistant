namespace Arcadia.Assistant.ExternalStorages.SharepointOnline.Contracts
{
    using System;
    using System.Linq.Expressions;

    using Abstractions;

    public interface ISharepointFieldsMapper
    {
        string GetSharepointField(Expression<Func<StorageItem, object>> property);
    }
}