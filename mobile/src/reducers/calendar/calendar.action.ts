import { CalendarEvents } from './calendar-events.model';
import { DayModel } from './calendar.model';

export interface LoadCalendarEventsFinished {
    type: 'LOAD-CALENDAR-EVENTS-FINISHED';
    calendarEvents: CalendarEvents[];
}

export const loadCalendarEventsFinished = (calendarEvents: CalendarEvents[]): LoadCalendarEventsFinished => ({ type: 'LOAD-CALENDAR-EVENTS-FINISHED', calendarEvents });

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

export type CalendarActions = LoadCalendarEventsFinished | 
    SelectCalendarDay | SelectCalendarMonth;