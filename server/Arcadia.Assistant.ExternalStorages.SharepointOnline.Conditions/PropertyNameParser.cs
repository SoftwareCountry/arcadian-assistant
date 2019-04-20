namespace Arcadia.Assistant.ExternalStorages.SharepointOnline.Conditions
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    using Arcadia.Assistant.ExternalStorages.Abstractions;

    public class PropertyNameParser
    {
        public void EnsureExpressionIsProperty(Expression<Func<StorageItem, object>> expression)
        {
            this.GetName(expression);
        }

        public string GetName(Expression<Func<StorageItem, object>> expression)
        {
            switch (expression.Body)
            {
                case MemberExpression member when member.Member.MemberType == MemberTypes.Property:
                    return member.Member.Name;

                case UnaryExpression unary when unary.Operand is MemberExpression operand && operand.Member.MemberType == MemberTypes.Property:
                    return operand.Member.Name;

                default:
                    throw new ArgumentException("Expression is not a property", nameof(expression));
            }
        }
    }
}