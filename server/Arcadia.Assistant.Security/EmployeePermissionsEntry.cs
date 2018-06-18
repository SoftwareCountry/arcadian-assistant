namespace Arcadia.Assistant.Security
{
    using System;

    [Flags]
    public enum EmployeePermissionsEntry
    {
        None = 0,
        ReadEmployeeInfo = 1 << 0,
        ReadEmployeeVacationsCounter = 1 << 1,
        ReadEmployeeDayoffsCounter = 1 << 2,
        ReadEmployeePhone = 1 << 3,
        ReadEmployeeCalendarEvents = 1 << 4,
        CreateCalendarEvents = 1 << 5,
        ApproveCalendarEvents = 1 << 6,
        RejectCalendarEvents = 1 << 7,
        CompleteCalendarEvents = 1 << 8,
        ProlongCalendarEvents = 1 << 9,
        CancelCalendarEvents = 1 << 10,
        EditCalendarEvents = 
            ApproveCalendarEvents |
            RejectCalendarEvents |
            CompleteCalendarEvents |
            ProlongCalendarEvents |
            CancelCalendarEvents,
        EditPendingCalendarEvents = 1 << 11,
    }
}