/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { CalendarEvent, CalendarEventStatus } from './calendar-event.model';
import { DayModel } from './calendar.model';
import { CalendarEvents } from './calendar-events.model';
import { Action } from 'redux';
import { Optional } from 'types';

//============================================================================
// - Actions
//============================================================================
export interface LoadCalendarEvents extends Action {
    type: 'LOAD-CALENDAR-EVENTS';
    employeeId: string;
    next?: Action[];
}

export interface LoadCalendarEventsFinished extends Action {
    type: 'LOAD-CALENDAR-EVENTS-FINISHED';
    calendarEvents: CalendarEvents;
    employeeId: string;
    next?: Action[];
}

export interface LoadCalendarEventsFailed extends Action {
    type: 'LOAD-CALENDAR-EVENTS-FAILED';
    employeeId: string;
    next?: Action[];
}

export interface CalendarEventSetNewStatus extends Action {
    type: 'CALENDAR-EVENT-NEW-STATUS';
    employeeId: string;
    calendarEvent: CalendarEvent;
    status: CalendarEventStatus;
}

export interface SelectCalendarDay extends Action {
    type: 'SELECT-CALENDAR-DAY';
    day: DayModel;
}

export interface NextCalendarPage extends Action {
    type: 'NEXT-CALENDAR-PAGE';
}

export interface PrevCalendarPage extends Action {
    type: 'PREV-CALENDAR-PAGE';
}

export interface ResetCalendarPages extends Action {
    type: 'RESET-CALENDAR-PAGES';
}

export enum CalendarSelectionModeType {
    SingleDay = 'SingleDay',
    Interval = 'Interval'
}

export interface CalendarSelectionMode extends Action {
    type: 'CALENDAR-SELECTION-MODE';
    selectionMode: CalendarSelectionModeType;
    color: Optional<string>;
}

export interface SelectIntervalsBySingleDaySelection extends Action {
    type: 'SELECT-INTERVALS-BY-SINGLE-DAY-SELECTION';
}

export interface DisableCalendarSelection extends Action {
    type: 'DISABLE-CALENDAR-SELECTION';
    disable: boolean;
}

export interface DisableSelectIntervalsBySingleDaySelection extends Action {
    type: 'DISABLE-SELECT-INTERVALS-BY-SINGLE-DAY-SELECTION';
    disable: boolean;
}

//============================================================================
// - Action Creators
//============================================================================
export const loadCalendarEvents = (employeeId: string, next?: Action[]): LoadCalendarEvents => {
    return {
        type: 'LOAD-CALENDAR-EVENTS',
        employeeId,
        next,
    };
};

export const loadCalendarEventsFinished = (calendarEvents: CalendarEvents, employeeId: string, next?: Action[]): LoadCalendarEventsFinished => {
    return {
        type: 'LOAD-CALENDAR-EVENTS-FINISHED',
        calendarEvents,
        employeeId,
        next: next,
    };
};

export const loadCalendarEventsFailed = (employeeId: string, next?: Action[]): LoadCalendarEventsFailed => {
    return {
        type: 'LOAD-CALENDAR-EVENTS-FAILED',
        employeeId,
        next: next,
    };
};

export const calendarEventSetNewStatus = (employeeId: string, calendarEvent: CalendarEvent, status: CalendarEventStatus): CalendarEventSetNewStatus => ({
    type: 'CALENDAR-EVENT-NEW-STATUS',
    calendarEvent,
    employeeId,
    status
});

export const selectCalendarDay = (day: DayModel): SelectCalendarDay => ({
    type: 'SELECT-CALENDAR-DAY', day
});

export const nextCalendarPage = (): NextCalendarPage => ({
    type: 'NEXT-CALENDAR-PAGE'
});

export const prevCalendarPage = (): PrevCalendarPage => ({
    type: 'PREV-CALENDAR-PAGE'
});

export const resetCalendarPages = (): ResetCalendarPages => ({
    type: 'RESET-CALENDAR-PAGES'
});

export const calendarSelectionMode = (selectionMode: CalendarSelectionModeType, color?: string): CalendarSelectionMode => ({
    type: 'CALENDAR-SELECTION-MODE',
    selectionMode,
    color
});

export const selectIntervalsBySingleDaySelection = (): SelectIntervalsBySingleDaySelection => ({
    type: 'SELECT-INTERVALS-BY-SINGLE-DAY-SELECTION'
});

export const disableCalendarSelection = (disable: boolean): DisableCalendarSelection => ({
    type: 'DISABLE-CALENDAR-SELECTION', disable
});

export const disableSelectIntervalsBySingleDaySelection = (disable: boolean): DisableSelectIntervalsBySingleDaySelection => ({
    type: 'DISABLE-SELECT-INTERVALS-BY-SINGLE-DAY-SELECTION',
    disable
});

//============================================================================
export type CalendarActions =
    LoadCalendarEvents
    | LoadCalendarEventsFinished
    | LoadCalendarEventsFailed
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
