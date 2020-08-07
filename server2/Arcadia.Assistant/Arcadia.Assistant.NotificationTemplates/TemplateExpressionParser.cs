namespace Arcadia.Assistant.NotificationTemplates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class TemplateExpressionParser
    {
        private readonly Regex ifBlockRegex =
            new Regex("(?<startIfGroup>{if:(?<startIfToken>.*?)}).*?(?<endIfGroup>{endif:(?<endIfToken>.*?)})");

        public string? Parse(string? template, IDictionary<string, string>? context)
        {
            if (template == null || context == null)
            {
                return template;
            }

            Match ifBlockMatch;
            while ((ifBlockMatch = this.ifBlockRegex.Match(template)).Success)
            {
                var startIfToken = ifBlockMatch.Groups["startIfToken"];
                var endIfToken = ifBlockMatch.Groups["endIfToken"];

                if (startIfToken.Value != endIfToken.Value)
                {
                    throw new ArgumentException("Wrong conditional tokens format", nameof(template));
                }

                var startIfGroup = ifBlockMatch.Groups["startIfGroup"];
                var endIfGroup = ifBlockMatch.Groups["endIfGroup"];

                if (!context.ContainsKey(startIfToken.Value))
                {
                    template = template.Remove(ifBlockMatch.Index, ifBlockMatch.Length);
                }
                else
                {
                    template = template
                        .Remove(endIfGroup.Index, endIfGroup.Length)
                        .Remove(startIfGroup.Index, startIfGroup.Length);
                }
            }

            var result = context.Aggregate(template, (res, val) => res.Replace($"{{{val.Key}}}", val.Value));
            return result;
        }
    }
}