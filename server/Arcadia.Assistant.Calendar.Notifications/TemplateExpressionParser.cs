namespace Arcadia.Assistant.Calendar.Notifications
{
    using System.Collections.Generic;
    using System.Linq;

    public static class TemplateExpressionParser
    {
        public static string ParseTemplateExpression(this string template, IDictionary<string, string> context)
        {
            var result = context.Aggregate(template, (res, val) => res.Replace($"{{{val.Key}}}", val.Value));
            return result;
        }
    }
}