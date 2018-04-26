import { Moment } from 'moment';
import { CalendarEvent } from './calendar-event.model';

export interface ConfirmClaimVacation {
    type: 'CONFIRM-VACATION';
    employeeId: string;
    startDate: Moment;
    endDate: Moment;
}

export const confirmVacation = (employeeId: string, startDate: Moment, endDate: Moment): ConfirmClaimVacation => ({ type: 'CONFIRM-VACATION', employeeId, startDate, endDate });

export interface CancelVacation {
    type: 'CANCEL-VACACTION';
    employeeId: string;
    calendarEvent: CalendarEvent;
}

export const сancelVacation = (employeeId: string, calendarEvent: CalendarEvent): CancelVacation => ({ type: 'CANCEL-VACACTION', calendarEvent, employeeId });

export interface ApproveVacation {
    type: 'APPROVE-VACATION';
    employeeId: string;
    calendarEvent: CalendarEvent;
}

export const approveVacation = (employeeId: string, calendarEvent: CalendarEvent): ApproveVacation => ({ type: 'APPROVE-VACATION', calendarEvent, employeeId });

export interface RejectVacation {
    type: 'REJECT-VACATION';
    employeeId: string;
    calendarEvent: CalendarEvent;
}

export const rejectVacation = (employeeId: string, calendarEvent: CalendarEvent): RejectVacation => ({ type: 'REJECT-VACATION', calendarEvent, employeeId });

export interface ConfirmVacationChange {
    type: 'CONFIRM-VACATION-CHANGE';
    employeeId: string;
    calendarEvent: CalendarEvent;
    startDate: Moment;
    endDate: Moment;
}

export const confirmVacationChange = (employeeId: string, calendarEvent: CalendarEvent, startDate: Moment, endDate: Moment): ConfirmVacationChange => 
    ({ type: 'CONFIRM-VACATION-CHANGE', employeeId, calendarEvent, startDate, endDate });