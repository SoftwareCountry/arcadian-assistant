namespace Arcadia.Assistant.ExternalStorages.Abstractions
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    public class PropertyNameParser
    {
        public void EnsureExpressionIsProperty<T>(Expression<Func<T, object>> expression)
        {
            this.GetName(expression);
        }

        public string GetName<T>(Expression<Func<T, object>> expression)
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