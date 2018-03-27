import { combineReducers } from 'redux';
import { combineEpics } from 'redux-observable';
import { loadUserEmployeeFinishedEpic$, calendarEventCreatedEpic$, intervalsBySingleDaySelectionEpic$, loadCalendarEventsEpic$, enableCalendarSelectionEpic$, loadCalendarEventsFinishedEpic$, disableCalendarSelectionEpic$ } from './calendar.epics';
import { DaysCountersState, daysCountersReducer } from './days-counters.reducer';
import { calendarEventsReducer, CalendarEventsState } from './calendar-events.reducer';
import { EventDialogState, eventDialogReducer } from './event-dialog/event-dialog.reducer';
import { openEventDialogEpic$, closeEventDialogEpic$, enableIntervalsBySingleSelectionEpic$ } from './event-dialog/event-dialog.epics';
import { sickLeaveSavedEpic$, sickLeaveCompletedEpic$, sickLeaveProlongedEpic$ } from './sick-leave.epics';

export interface CalendarState {
    daysCounters: DaysCountersState;
    calendarEvents: CalendarEventsState;
    eventDialog: EventDialogState;
}

export const calendarEpics = combineEpics(
    loadUserEmployeeFinishedEpic$ as any,
    loadCalendarEventsEpic$ as any,
    loadCalendarEventsFinishedEpic$ as any,
    calendarEventCreatedEpic$ as any,
    sickLeaveSavedEpic$ as any,
    sickLeaveCompletedEpic$ as any,
    sickLeaveProlongedEpic$ as any,
    intervalsBySingleDaySelectionEpic$ as any,
    openEventDialogEpic$ as any,
    closeEventDialogEpic$ as any,
    enableCalendarSelectionEpic$ as any,
    enableIntervalsBySingleSelectionEpic$ as any,
    disableCalendarSelectionEpic$ as any);

export const calendarReducer = combineReducers<CalendarState>({
    daysCounters: daysCountersReducer,
    calendarEvents: calendarEventsReducer,
    eventDialog: eventDialogReducer
});
