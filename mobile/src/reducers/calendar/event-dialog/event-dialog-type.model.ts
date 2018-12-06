export enum EventDialogType {
    ClaimSickLeave = 'ClaimSickLeave',
    ConfirmStartDateSickLeave = 'ConfirmStartDateSickLeave',
    EditSickLeave = 'EditSickLeave',
    ProlongSickLeave = 'ProlongSickLeave',
    CancelSickLeave = 'CancelSickLeave',

    RequestVacation = 'RequestVacation',
    ConfirmStartDateVacation = 'ConfirmStartDateVacation',
    EditVacation = 'EditVacation',
    CancelVacation = 'CancelVacation',
    ChangeVacationStartDate = 'ChangeVacationStartDate',
    ChangeVacationEndDate = 'ChangeVacationEndDate',
    VacationRequested = 'VacationRequested',

    ProcessDayoff = 'ProcessDayoff',
    ChooseTypeDayoff = 'ChooseTypeDayoff',
    ConfirmDayoffStartDate = 'ConfirmDayoffStartDate',
    EditDayoff = 'EditDayoff',
    DayoffRequested = 'DayoffRequested',
}