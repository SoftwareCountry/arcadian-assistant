import { combineReducers } from 'redux';
import { combineEpics } from 'redux-observable';
import { loadUserEmployeeFinishedEpic$, calendarEventCreatedEpic$, intervalsBySingleDaySelectionEpic$, loadCalendarEventsEpic$, loadCalendarEventsFinishedEpic$, calendarSelectionModeEpic$ } from './calendar.epics';
import { DaysCountersState, daysCountersReducer } from './days-counters.reducer';
import { calendarEventsReducer, CalendarEventsState } from './calendar-events.reducer';
import { EventDialogState, eventDialogReducer } from './event-dialog/event-dialog.reducer';
import { openEventDialogEpic$, closeEventDialogEpic$ } from './event-dialog/event-dialog.epics';
import { sickLeaveSavedEpic$, sickLeaveCompletedEpic$, sickLeaveProlongedEpic$ } from './sick-leave.epics';
import { vacationSavedEpic$, vacationCanceledEpic$ } from './vacation.epics';

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
    vacationSavedEpic$ as any,
    vacationCanceledEpic$ as any,
    intervalsBySingleDaySelectionEpic$ as any,
    openEventDialogEpic$ as any,
    closeEventDialogEpic$ as any,
    calendarSelectionModeEpic$ as any);

export const calendarReducer = combineReducers<CalendarState>({
    daysCounters: daysCountersReducer,
    calendarEvents: calendarEventsReducer,
    eventDialog: eventDialogReducer
});
