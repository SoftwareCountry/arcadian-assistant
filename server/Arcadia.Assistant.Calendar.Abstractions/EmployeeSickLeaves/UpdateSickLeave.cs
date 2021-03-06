﻿namespace Arcadia.Assistant.Calendar.Abstractions.EmployeeSickLeaves
{
    using System;

    public class UpdateSickLeave
    {
        public UpdateSickLeave(
            CalendarEvent newEvent,
            CalendarEvent oldEvent,
            string updatedBy,
            DateTimeOffset timestamp)
        {
            this.NewEvent = newEvent;
            this.OldEvent = oldEvent;
            this.UpdatedBy = updatedBy;
            this.Timestamp = timestamp;
        }

        public CalendarEvent NewEvent { get; }

        public CalendarEvent OldEvent { get; }

        public string UpdatedBy { get; }

        public DateTimeOffset Timestamp { get; }

        public abstract class Response
        {
        }

        public class Success : Response
        {
            public Success(CalendarEvent newEvent, CalendarEvent oldEvent, string updatedBy, DateTimeOffset timestamp)
            {
                this.NewEvent = newEvent;
                this.OldEvent = oldEvent;
                this.UpdatedBy = updatedBy;
                this.Timestamp = timestamp;
            }

            public CalendarEvent NewEvent { get; }

            public CalendarEvent OldEvent { get; }

            public string UpdatedBy { get; }

            public DateTimeOffset Timestamp { get; }
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