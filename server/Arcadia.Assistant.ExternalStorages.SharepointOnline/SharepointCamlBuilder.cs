namespace Arcadia.Assistant.ExternalStorages.SharepointOnline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Arcadia.Assistant.ExternalStorages.Abstractions;
    using Arcadia.Assistant.ExternalStorages.SharepointOnline.Conditions;
    using Arcadia.Assistant.ExternalStorages.SharepointOnline.Contracts;

    using Microsoft.SharePoint.Client;

    public class SharepointCamlBuilder : ISharepointCamlBuilder
    {
        private readonly ISharepointFieldsMapper fieldsMapper;

        public SharepointCamlBuilder(ISharepointFieldsMapper fieldsMapper)
        {
            this.fieldsMapper = fieldsMapper;
        }

        public CamlQuery GetCamlQuery(IEnumerable<ICondition> conditions = null)
        {
            var camlQuery = CamlQuery.CreateAllItemsQuery();

            if (conditions == null)
            {
                return camlQuery;
            }

            var conditionsXml = conditions
                .Select(this.GetConditionCamlXml)
                .Aggregate(new StringBuilder(), (result, current) => result.AppendLine(current))
                .ToString();

            camlQuery.ViewXml = $"<View><Query><Where>{conditionsXml}</Where></Query></View>";

            return camlQuery;
        }

        private string GetConditionCamlXml(ICondition condition)
        {
            switch (condition)
            {
                case SharepointEqualCondition equalCondition:
                    return this.GetEqualCamlCondition(equalCondition);

                default:
                    throw new ArgumentException($"Not supported condition type: {condition.GetType()}", nameof(condition));
            }
        }

        private string GetEqualCamlCondition(BaseSharepointCondition equalCondition)
        {
            var sharepointField = this.fieldsMapper.GetSharepointField(equalCondition.Property);
            return $"<Eq><FieldRef Name='{sharepointField.Name}'/><Value Type='{sharepointField.ValueType}'>{equalCondition.Value}</Value></Eq>";
        }
    }
}