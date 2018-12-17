import { Moment } from 'moment';
import { CalendarEvent } from './calendar-event.model';
import { Action } from 'redux';

export interface ConfirmClaimVacation extends Action {
    type: 'CONFIRM-VACATION';
    employeeId: string;
    startDate: Moment;
    endDate: Moment;
}

export const confirmVacation = (employeeId: string, startDate: Moment, endDate: Moment): ConfirmClaimVacation => ({
    type: 'CONFIRM-VACATION',
    employeeId,
    startDate,
    endDate
});

export interface CancelVacation extends Action {
    type: 'CANCEL-VACACTION';
    employeeId: string;
    calendarEvent: CalendarEvent;
}

export const cancelVacation = (employeeId: string, calendarEvent: CalendarEvent): CancelVacation => ({
    type: 'CANCEL-VACACTION',
    calendarEvent,
    employeeId
});

export interface ConfirmVacationChange extends Action {
    type: 'CONFIRM-VACATION-CHANGE';
    employeeId: string;
    calendarEvent: CalendarEvent;
    startDate: Moment;
    endDate: Moment;
}

export const confirmVacationChange = (employeeId: string, calendarEvent: CalendarEvent, startDate: Moment, endDate: Moment): ConfirmVacationChange =>
    ({ type: 'CONFIRM-VACATION-CHANGE', employeeId, calendarEvent, startDate, endDate });

export type VacationActions = ConfirmClaimVacation | CancelVacation | ConfirmVacationChange;
