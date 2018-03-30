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

export interface ChangeVacation {
    type: 'CHANGE-VACACTION';
}

export const changeVacation = (): ChangeVacation => ({ type: 'CHANGE-VACACTION' });