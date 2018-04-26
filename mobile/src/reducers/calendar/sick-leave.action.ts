import { Moment } from 'moment';
import { Employee } from '../organization/employee.model';
import { CalendarEvent } from './calendar-event.model';

export interface ConfirmClaimSickLeave {
    type: 'CONFIRM-CLAIM-SICK-LEAVE';
    employeeId: string;
    startDate: Moment;
    endDate: Moment;
}

export const confirmSickLeave = (employeeId: string, startDate: Moment, endDate: Moment): ConfirmClaimSickLeave => ({ type: 'CONFIRM-CLAIM-SICK-LEAVE', employeeId, startDate, endDate });

export interface CompleteSickLeave {
    type: 'COMPLETE-SICK-LEAVE';
    employeeId: string;
    calendarEvent: CalendarEvent;
}

export const completeSickLeave = (employeeId: string, calendarEvent: CalendarEvent): CompleteSickLeave => ({ type: 'COMPLETE-SICK-LEAVE', employeeId, calendarEvent });

export interface ConfirmProlongSickLeave {
    type: 'CONFIRM-PROLONG-SICK-LEAVE';
    employeeId: string;
    calendarEvent: CalendarEvent;
    prolongedEndDate: Moment;
}

export const confirmProlongSickLeave = (employeeId: string, calendarEvent: CalendarEvent, prolongedEndDate: Moment): ConfirmProlongSickLeave => 
    ({ type: 'CONFIRM-PROLONG-SICK-LEAVE', employeeId, calendarEvent, prolongedEndDate });

export interface CancelSickLeave {
    type: 'CANCEL-SICK-LEAVE';
    employeeId: string;
    calendarEvent: CalendarEvent;
}

export const cancelSickLeave = (employeeId: string, calendarEvent: CalendarEvent): CancelSickLeave => ({ type: 'CANCEL-SICK-LEAVE', employeeId, calendarEvent });

export type SickLeaveActions =  ConfirmClaimSickLeave | CompleteSickLeave | ConfirmProlongSickLeave | CancelSickLeave;