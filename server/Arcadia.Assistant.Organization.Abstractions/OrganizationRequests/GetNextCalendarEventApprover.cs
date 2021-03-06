﻿namespace Arcadia.Assistant.Organization.Abstractions.OrganizationRequests
{
    using System.Collections.Generic;

    public class GetNextCalendarEventApprover
    {
        public GetNextCalendarEventApprover(
            string employeeId,
            IEnumerable<string> existingApprovals,
            string eventType,
            IEnumerable<string> skippedApprovers = null)
        {
            this.EmployeeId = employeeId;
            this.ExistingApprovals = existingApprovals;
            this.EventType = eventType;
            this.SkippedApprovers = skippedApprovers;
        }

        public string EmployeeId { get; }

        public IEnumerable<string> ExistingApprovals { get; }

        public IEnumerable<string> SkippedApprovers { get; }

        public string EventType { get; }

        public abstract class Response
        {
        }

        public class SuccessResponse : Response
        {
            public SuccessResponse(string nextApproverEmployeeId)
            {
                this.NextApproverEmployeeId = nextApproverEmployeeId;
            }

            public string NextApproverEmployeeId { get; }
        }

        public class ErrorResponse : Response
        {
            public ErrorResponse(string message)
            {
                this.Message = message;
            }

            public string Message { get; }
        }
    }
}