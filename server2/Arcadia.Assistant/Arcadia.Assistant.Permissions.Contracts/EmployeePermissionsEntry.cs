namespace Arcadia.Assistant.Permissions.Contracts
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
        CompleteSickLeave = 1 << 8,
        ProlongSickLeave = 1 << 9,
        CancelPendingCalendarEvents = 1 << 10,
        EditPendingCalendarEvents = 1 << 11,
        CancelApprovedCalendarEvents = 1 << 12
    }
}