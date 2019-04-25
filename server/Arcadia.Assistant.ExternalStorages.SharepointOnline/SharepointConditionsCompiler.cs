namespace Arcadia.Assistant.ExternalStorages.SharepointOnline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    using Arcadia.Assistant.ExternalStorages.Abstractions;
    using Arcadia.Assistant.ExternalStorages.SharepointOnline.Contracts;

    public class SharepointConditionsCompiler : ISharepointConditionsCompiler
    {
        private readonly ISharepointFieldsMapper fieldsMapper;

        public SharepointConditionsCompiler(ISharepointFieldsMapper fieldsMapper)
        {
            this.fieldsMapper = fieldsMapper;
        }

        public string CompileConditions(IEnumerable<ICondition> conditions = null)
        {
            if (conditions == null)
            {
                return null;
            }

            var compiledConditions = conditions
                .Select(this.CompileCondition);

            return $"$filter={HttpUtility.UrlEncode(string.Join(" and ", compiledConditions))}";
        }

        private string CompileCondition(ICondition condition)
        {
            switch (condition)
            {
                case EqualCondition equalCondition:
                    return this.GetEqualCamlCondition(equalCondition);

                default:
                    throw new ArgumentException($"Not supported condition type: {condition.GetType()}", nameof(condition));
            }
        }

        private string GetEqualCamlCondition(BaseCondition equalCondition)
        {
            var sharepointField = this.fieldsMapper.GetSharepointField(equalCondition.Property);
            object fieldValue;

            if (equalCondition.Value is DateTime dateTimeValue)
            {
                throw new NotImplementedException("Filter by datetime is not supported yet.");
            }
            else
            {
                fieldValue = equalCondition.Value;
            }

            return $"{sharepointField} eq '{fieldValue}'";
        }
    }
}