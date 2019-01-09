namespace Arcadia.Assistant.InboxEmail
{
    using System;
    using System.Collections.Generic;

    using Arcadia.Assistant.InboxEmail.Abstractions;

    public class GetInboxEmails
    {
        public GetInboxEmails(EmailSearchQuery emailSearchQuery)
        {
            this.EmailSearchQuery = emailSearchQuery;
        }

        public EmailSearchQuery EmailSearchQuery { get; }

        public abstract class Response
        {
        }

        public class Success : Response
        {
            public Success(IEnumerable<Email> emails)
            {
                this.Emails = emails;
            }

            public IEnumerable<Email> Emails { get; }
        }

        public class Error : Response
        {
            public Error(Exception exception)
            {
                this.Exception = exception;
            }

            public Exception Exception { get; }
        }
    }
}