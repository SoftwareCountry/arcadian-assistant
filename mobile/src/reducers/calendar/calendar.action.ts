import { Map } from 'immutable';
import { CalendarEvent, CalendarEventStatus } from './calendar-event.model';
import { DayModel, CalendarSelection } from './calendar.model';
import { SickLeaveActions } from './sick-leave.action';
import { CalendarEvents, PendingRequests } from './calendar-events.model';

export interface LoadCalendarEvents {
    type: 'LOAD-CALENDAR-EVENTS';
    employeeId: string;
}

export const loadCalendarEvents = (employeeId: string): LoadCalendarEvents => ({ type: 'LOAD-CALENDAR-EVENTS', employeeId });

export interface LoadCalendarEventsFinished {
    type: 'LOAD-CALENDAR-EVENTS-FINISHED';
    calendarEvents: CalendarEvents;
    employeeId: string;
}

export const loadCalendarEventsFinished = (calendarEvents: CalendarEvents, employeeId: string): LoadCalendarEventsFinished => ({ type: 'LOAD-CALENDAR-EVENTS-FINISHED', calendarEvents, employeeId });

export interface LoadPendingRequests {
    type: 'LOAD-PENDING-REQUESTS';
}

export const loadPendingRequests = (): LoadPendingRequests => ({ type: 'LOAD-PENDING-REQUESTS' });

export interface LoadPendingRequestsFinished {
    type: 'LOAD-PENDING-REQUESTS-FINISHED';
    requests: Map<string, CalendarEvent[]>;
}

export const loadPendingRequestsFinished = (requests: Map<string, CalendarEvent[]>): LoadPendingRequestsFinished => ({ type: 'LOAD-PENDING-REQUESTS-FINISHED', requests });

export interface CalendarEventSetNewStatus {
    type: 'CALENDAR-EVENT-NEW-STATUS';
    employeeId: string;
    calendarEvent: CalendarEvent;
    status: CalendarEventStatus;
}

export const calendarEventSetNewStatus = (employeeId: string, calendarEvent: CalendarEvent, status: CalendarEventStatus): CalendarEventSetNewStatus => ({ type: 'CALENDAR-EVENT-NEW-STATUS', calendarEvent, employeeId, status });

export interface SelectCalendarDay {
    type: 'SELECT-CALENDAR-DAY';
    day: DayModel;
}

export const selectCalendarDay = (day: DayModel): SelectCalendarDay => ({ type: 'SELECT-CALENDAR-DAY', day });

export interface NextCalendarPage {
    type: 'NEXT-CALENDAR-PAGE';
}

export const nextCalendarPage = (): NextCalendarPage => ({ type: 'NEXT-CALENDAR-PAGE' });

export interface PrevCalendarPage {
    type: 'PREV-CALENDAR-PAGE';
}

export const prevCalendarPage = (): PrevCalendarPage => ({ type: 'PREV-CALENDAR-PAGE' });

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

export interface SelectIntervalsBySingleDaySelection {
    type: 'SELECT-INTERVALS-BY-SINGLE-DAY-SELECTION';
}

export const selectIntervalsBySingleDaySelection = (): SelectIntervalsBySingleDaySelection => ({ type: 'SELECT-INTERVALS-BY-SINGLE-DAY-SELECTION' });

export interface DisableCalendarSelection {
    type: 'DISABLE-CALENDAR-SELECTION';
    disable: boolean;
}

export const disableCalendarSelection = (disable: boolean): DisableCalendarSelection => 
    ({ type: 'DISABLE-CALENDAR-SELECTION', disable });

export interface DisableSelectIntervalsBySingleDaySelection {
    type: 'DISABLE-SELECT-INTERVALS-BY-SINGLE-DAY-SELECTION';
    disable: boolean;
}

export const disableSelectIntervalsBySingleDaySelection = (disable: boolean): DisableSelectIntervalsBySingleDaySelection => ({ type: 'DISABLE-SELECT-INTERVALS-BY-SINGLE-DAY-SELECTION', disable });

export type CalendarActions = LoadCalendarEventsFinished | LoadPendingRequestsFinished | SelectCalendarDay | NextCalendarPage | PrevCalendarPage |
    CalendarSelectionMode | SelectIntervalsBySingleDaySelection | DisableCalendarSelection | DisableSelectIntervalsBySingleDaySelection | CalendarEventSetNewStatus;
