﻿namespace Arcadia.Assistant.Feeds.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class GetMessages
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public GetMessages()
        {
        }

        public GetMessages(DateTime? fromDate, DateTime? toDate)
        {
            this.FromDate = fromDate;
            this.ToDate = toDate;
        }

        public sealed class Response
        {
            public Response(IEnumerable<Message> messages)
            {
                this.Messages = messages.ToList();
            }

            public IReadOnlyCollection<Message> Messages { get; }
        }
    }
}