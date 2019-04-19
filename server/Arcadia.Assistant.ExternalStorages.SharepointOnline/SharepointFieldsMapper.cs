namespace Arcadia.Assistant.ExternalStorages.SharepointOnline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Arcadia.Assistant.ExternalStorages.Abstractions;
    using Arcadia.Assistant.ExternalStorages.SharepointOnline.Contracts;

    public class SharepointFieldsMapper : ISharepointFieldsMapper
    {
        private readonly Dictionary<string, SharepointField> fieldsByPropertyName;
        private readonly Dictionary<string, Expression<Func<StorageItem, object>>> propertiesByFieldName;

        public SharepointFieldsMapper(params SharepointFieldMapping[] fields)
        {
            this.fieldsByPropertyName = fields
                .ToDictionary(f => this.GetPropertyName(f.Property), f => f.Field);

            this.propertiesByFieldName = fields
                .ToDictionary(f => f.Field.Name, f => f.Property);
        }

        public static SharepointFieldMapping CreateMapping(Expression<Func<StorageItem, object>> property, string fieldName, string fieldValueType)
        {
            return new SharepointFieldMapping(property, new SharepointField(fieldName, fieldValueType));
        }

        public SharepointField GetSharepointField(Expression<Func<StorageItem, object>> property)
        {
            var propertyName = this.GetPropertyName(property);

            if (this.fieldsByPropertyName.TryGetValue(propertyName, out var field))
            {
                return field;
            }

            throw new ArgumentException($"Mapping for property '{propertyName}' doesn't exist");
        }

        public Expression<Func<StorageItem, object>> GetProperty(string fieldName)
        {
            if (this.propertiesByFieldName.TryGetValue(fieldName, out var property))
            {
                return property;
            }

            throw new ArgumentException($"Mapping for field name '{fieldName}' doesn't exist");
        }

        private string GetPropertyName(Expression<Func<StorageItem, object>> property)
        {
            switch (property.Body)
            {
                case MemberExpression member:
                    return member.Member.Name;

                case UnaryExpression unary when unary.Operand is MemberExpression operand:
                    return operand.Member.Name;

                default:
                    throw new ArgumentException("Wrong expression type", nameof(property));
            }
        }

        public struct SharepointFieldMapping
        {
            public SharepointFieldMapping(Expression<Func<StorageItem, object>> property, SharepointField field)
            {
                this.Property = property;
                this.Field = field;
            }

            public Expression<Func<StorageItem, object>> Property { get; }

            public SharepointField Field { get; }
        }
    }
}