import { CalendarEvents } from './calendar-events';
import { DayModel } from '../../calendar/calendar-page';

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

export type CalendarActions = LoadCalendarEventsFinished | SelectCalendarDay;