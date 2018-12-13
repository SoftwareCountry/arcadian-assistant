import { CalendarEvent, CalendarEventStatus } from './calendar-event.model';
import { DayModel } from './calendar.model';
import { CalendarEvents } from './calendar-events.model';
import { Action } from 'redux';
import { Optional } from 'types';

export interface LoadCalendarEvents extends Action {
    type: 'LOAD-CALENDAR-EVENTS';
    employeeId: string;
    next?: Action[];
}

export const loadCalendarEvents = (employeeId: string, next?: Action[]): LoadCalendarEvents => {
    return {
        type: 'LOAD-CALENDAR-EVENTS',
        employeeId,
        next,
    };
};

export interface LoadCalendarEventsFinished extends Action {
    type: 'LOAD-CALENDAR-EVENTS-FINISHED';
    calendarEvents: CalendarEvents;
    employeeId: string;
    next?: Action[];
}

export const loadCalendarEventsFinished = (calendarEvents: CalendarEvents, employeeId: string, next?: Action[]): LoadCalendarEventsFinished => {
    return {
        type: 'LOAD-CALENDAR-EVENTS-FINISHED',
        calendarEvents,
        employeeId,
        next: next,
    };
};

export interface CalendarEventSetNewStatus extends Action {
    type: 'CALENDAR-EVENT-NEW-STATUS';
    employeeId: string;
    calendarEvent: CalendarEvent;
    status: CalendarEventStatus;
}

export const calendarEventSetNewStatus = (employeeId: string, calendarEvent: CalendarEvent, status: CalendarEventStatus): CalendarEventSetNewStatus => ({
    type: 'CALENDAR-EVENT-NEW-STATUS',
    calendarEvent,
    employeeId,
    status
});

export interface SelectCalendarDay extends Action {
    type: 'SELECT-CALENDAR-DAY';
    day: DayModel;
}

export const selectCalendarDay = (day: DayModel): SelectCalendarDay => ({ type: 'SELECT-CALENDAR-DAY', day });

export interface NextCalendarPage extends Action {
    type: 'NEXT-CALENDAR-PAGE';
}

export const nextCalendarPage = (): NextCalendarPage => ({ type: 'NEXT-CALENDAR-PAGE' });

export interface PrevCalendarPage extends Action {
    type: 'PREV-CALENDAR-PAGE';
}

export const prevCalendarPage = (): PrevCalendarPage => ({ type: 'PREV-CALENDAR-PAGE' });

export interface ResetCalendarPages {
    type: 'RESET-CALENDAR-PAGES';
}

export const resetCalendarPages = (): ResetCalendarPages => ({ type: 'RESET-CALENDAR-PAGES' });

export enum CalendarSelectionModeType {
    SingleDay = 'SingleDay',
    Interval = 'Interval'
}

export interface CalendarSelectionMode extends Action {
    type: 'CALENDAR-SELECTION-MODE';
    selectionMode: CalendarSelectionModeType;
    color: Optional<string>;
}

export const calendarSelectionMode = (selectionMode: CalendarSelectionModeType, color?: string): CalendarSelectionMode => ({
    type: 'CALENDAR-SELECTION-MODE',
    selectionMode,
    color
});

export interface SelectIntervalsBySingleDaySelection extends Action {
    type: 'SELECT-INTERVALS-BY-SINGLE-DAY-SELECTION';
}

export const selectIntervalsBySingleDaySelection = (): SelectIntervalsBySingleDaySelection => ({ type: 'SELECT-INTERVALS-BY-SINGLE-DAY-SELECTION' });

export interface DisableCalendarSelection {
    type: 'DISABLE-CALENDAR-SELECTION';
    disable: boolean;
}

export const disableCalendarSelection = (disable: boolean): DisableCalendarSelection =>
    ({ type: 'DISABLE-CALENDAR-SELECTION', disable });

export interface DisableSelectIntervalsBySingleDaySelection extends Action {
    type: 'DISABLE-SELECT-INTERVALS-BY-SINGLE-DAY-SELECTION';
    disable: boolean;
}

export const disableSelectIntervalsBySingleDaySelection = (disable: boolean): DisableSelectIntervalsBySingleDaySelection => ({
    type: 'DISABLE-SELECT-INTERVALS-BY-SINGLE-DAY-SELECTION',
    disable
});

export type CalendarActions =
    LoadCalendarEventsFinished
    | SelectCalendarDay
    | NextCalendarPage
    | PrevCalendarPage
    | ResetCalendarPages
    |
    CalendarSelectionMode
    | SelectIntervalsBySingleDaySelection
    | DisableCalendarSelection
    | DisableSelectIntervalsBySingleDaySelection
    | CalendarEventSetNewStatus;
