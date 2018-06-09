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
        ApproveEmployeeCalendarEvents = 1 << 6,
        RejectEmployeeCalendarEvents = 1 << 7,
    }
}