namespace Arcadia.Assistant.Organization.Abstractions.OrganizationRequests
{
    using System;
    using System.Collections.Generic;

    public class FeedsQuery
    {
        public DateTime From { get; private set; }

        public DateTime To { get; private set; }

        public static FeedsQuery Create() 
        {
            return new FeedsQuery();
        }

        public FeedsQuery InDateRange(DateTime from, DateTime to) 
        {
            var obj = this.Clone();
            obj.From = from;
            obj.To = to;
            return obj;
        }

        private FeedsQuery Clone() 
        {
            var query = new FeedsQuery();
            query.From = this.From;
            query.To = this.To;
            return query;
        }
    }
}
