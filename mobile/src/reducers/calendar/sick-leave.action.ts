import { Moment } from 'moment';
import { Employee } from '../organization/employee.model';
import { CalendarEvents } from './calendar-events.model';

export interface ClaimSickLeave {
    type: 'CLAIM-SICK-LEAVE';
}

export const claimSickLeave = (): ClaimSickLeave => ({ type: 'CLAIM-SICK-LEAVE' });

export interface ConfirmStartDateSickLeave {
    type: 'CONFIRM-START-DATE-SICK-LEAVE';
    startDate: Moment;
}

export const confirmStartDateSickLeave = (startDate: Moment): ConfirmStartDateSickLeave => ({ type: 'CONFIRM-START-DATE-SICK-LEAVE', startDate });

export interface BackToClaimSickLeave {
    type: 'BACK-TO-CLAIM-SICK-LEAVE';
    startDate: Moment;
}

export const backToClaimSickLeave = (startDate: Moment): BackToClaimSickLeave => ({ type: 'BACK-TO-CLAIM-SICK-LEAVE', startDate });

export interface ConfirmClaimSickLeave {
    type: 'CONFIRM-CLAIM-SICK-LEAVE';
    employee: Employee;
    calendarEvents: CalendarEvents;
}

export const confirmSickLeave = (employee: Employee, calendarEvents: CalendarEvents): ConfirmClaimSickLeave => ({ type: 'CONFIRM-CLAIM-SICK-LEAVE', employee, calendarEvents });

export interface EditSickLeave {
    type: 'EDIT-SICK-LEAVE';
}

export const editSickLeave = (): EditSickLeave => ({ type: 'EDIT-SICK-LEAVE' });

export interface CompleteSickLeave {
    type: 'COMPLETE-SICK-LEAVE';
}

export const completeSickLeave = (): CompleteSickLeave => ({ type: 'COMPLETE-SICK-LEAVE' });

export interface ProlongSickLeave {
    type: 'PROLONG-SICK-LEAVE';
}

export const prolongSickLeave = (): ProlongSickLeave => ({ type: 'PROLONG-SICK-LEAVE' });

export interface ConfirmProlongSickLeave {
    type: 'CONFIRM-PROLONG-SICK-LEAVE';
}

export const confirmProlongSickLeave = (): ConfirmProlongSickLeave => ({ type: 'CONFIRM-PROLONG-SICK-LEAVE' });

export interface SickLeaveSaved {
    type: 'SICK-LEAVE-SAVED';
    employee: Employee;
}

export const sickLeaveSaved = (employee: Employee): SickLeaveSaved => ({ type: 'SICK-LEAVE-SAVED', employee });

export type SickLeaveActions = ClaimSickLeave | ConfirmStartDateSickLeave | BackToClaimSickLeave | ConfirmClaimSickLeave | SickLeaveSaved |
    EditSickLeave | CompleteSickLeave | ProlongSickLeave | ConfirmProlongSickLeave;