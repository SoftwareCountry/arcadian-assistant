import { combineReducers } from 'redux';
import { combineEpics } from 'redux-observable';
import { loadCalendarEventsFinishedEpic$, calendarEventsSavedEpic$, calendarEventCreatedEpic$ } from './calendar.epics';
import { DaysCountersState, daysCountersReducer } from './days-counters.reducer';
import { calendarEventsReducer, CalendarEventsState } from './calendar-events.reducer';
import { eventDialogReducer, EventDialogState } from './event-dialog.reducer';
import { openEventDialogEpic$, closeEventDialogEpic$ } from './event-dialog.epics';

export interface CalendarState {
    daysCounters: DaysCountersState;
    calendarEvents: CalendarEventsState;
    eventDialog: EventDialogState;
}

export const calendarEpics = combineEpics(
    loadCalendarEventsFinishedEpic$ as any, 
    calendarEventCreatedEpic$ as any,
    calendarEventsSavedEpic$ as any,
    openEventDialogEpic$ as any,
    closeEventDialogEpic$ as any);

export const calendarReducer = combineReducers<CalendarState>({
    daysCounters: daysCountersReducer,
    calendarEvents: calendarEventsReducer,
    eventDialog: eventDialogReducer
});
