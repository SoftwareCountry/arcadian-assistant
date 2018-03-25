import { CalendarEvent } from './calendar-event.model';
import { DayModel } from './calendar.model';
import { SickLeaveActions } from './sick-leave.action';
import { CalendarEvents } from './calendar-events.model';

export interface LoadCalendarEvents {
    type: 'LOAD-CALENDAR-EVENTS';
    employeeId: string;
}

export const loadCalendarEvents = (employeeId: string): LoadCalendarEvents => ({ type: 'LOAD-CALENDAR-EVENTS', employeeId });

export interface LoadCalendarEventsFinished {
    type: 'LOAD-CALENDAR-EVENTS-FINISHED';
    calendarEvents: CalendarEvents;
}

export const loadCalendarEventsFinished = (calendarEvents: CalendarEvents): LoadCalendarEventsFinished => ({ type: 'LOAD-CALENDAR-EVENTS-FINISHED', calendarEvents });

export interface CalendarEventCreated {
    type: 'CALENDAR-EVENT-CREATED';
    calendarEvent: CalendarEvent;
}

export const calendarEventCreated = (calendarEvent: CalendarEvent): CalendarEventCreated => ({ type: 'CALENDAR-EVENT-CREATED', calendarEvent });

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

export enum CalendarSelectionModeType {
    SingleDay = 'SingleDay',
    Interval = 'Interval'
}

export interface CalendarSelectionMode {
    type: 'CALENDAR-SELECTION-MODE';
    selectionMode: CalendarSelectionModeType;
    color: string;
}

export const calendarSelectionMode = (selectionMode: CalendarSelectionModeType, color: string = null): CalendarSelectionMode => ({ type: 'CALENDAR-SELECTION-MODE', selectionMode, color });

export interface IntervalsBySingleDaySelection {
    type: 'INTERVALS-BY-SINGLE-DAY-SELECTION';
}

export const intervalsBySingleDaySelection = (): IntervalsBySingleDaySelection => ({ type: 'INTERVALS-BY-SINGLE-DAY-SELECTION' });

export type CalendarActions = LoadCalendarEventsFinished | CalendarEventCreated |
    SelectCalendarDay | SelectCalendarMonth |
    CalendarSelectionMode |
    IntervalsBySingleDaySelection;
