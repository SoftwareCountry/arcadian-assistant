import { CalendarEvents } from './calendar-events.model';
import { DayModel } from './calendar.model';
import { SickLeaveActions } from './sick-leave.action';

export interface LoadCalendarEventsFinished {
    type: 'LOAD-CALENDAR-EVENTS-FINISHED';
    calendarEvents: CalendarEvents[];
}

export const loadCalendarEventsFinished = (calendarEvents: CalendarEvents[]): LoadCalendarEventsFinished => ({ type: 'LOAD-CALENDAR-EVENTS-FINISHED', calendarEvents });

export interface CalendarEventCreated {
    type: 'CALENDAR-EVENT-CREATED';
    calendarEvent: CalendarEvents;
}

export const calendarEventCreated = (calendarEvent: CalendarEvents): CalendarEventCreated => ({ type: 'CALENDAR-EVENT-CREATED', calendarEvent });

export interface SelectCalendarDay {
    type: 'SELECT-CALENDAR-DAY';
    day: DayModel;
}

export const selectCalendarDay = (day: DayModel): SelectCalendarDay => ({ type: 'SELECT-CALENDAR-DAY', day });

export interface SelectCalendarMonth {
    type: 'SELECT-CALENDAR-MONTH';
    month: number;
    year: number;
}

export const selectCalendarMonth = (month: number, year: number): SelectCalendarMonth => ({ type: 'SELECT-CALENDAR-MONTH', month, year });

export interface CancelDialog {
    type: 'CANCEL-CALENDAR-DIALOG';
}

export const cancelDialog = (): CancelDialog => ({ type: 'CANCEL-CALENDAR-DIALOG' });

export type CalendarActions = LoadCalendarEventsFinished | CalendarEventCreated |
    SelectCalendarDay | SelectCalendarMonth |
    SickLeaveActions |
    CancelDialog;