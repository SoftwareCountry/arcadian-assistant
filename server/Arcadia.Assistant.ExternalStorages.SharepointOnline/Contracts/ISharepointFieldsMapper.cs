namespace Arcadia.Assistant.ExternalStorages.SharepointOnline.Contracts
{
    using System;
    using System.Linq.Expressions;

    using Arcadia.Assistant.ExternalStorages.Abstractions;

    public interface ISharepointFieldsMapper
    {
        SharepointField GetSharepointField(Expression<Func<StorageItem, object>> property);

        Expression<Func<StorageItem, object>> GetProperty(string sharepointFieldName);
    }
}