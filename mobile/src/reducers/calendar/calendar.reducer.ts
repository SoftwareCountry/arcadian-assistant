import { combineReducers } from 'redux';
import { combineEpics } from 'redux-observable';
import { loadUserEmployeeFinishedEpic$, intervalsBySingleDaySelectionEpic$, 
        loadCalendarEventsEpic$, loadCalendarEventsFinishedEpic$, calendarSelectionModeEpic$, 
        calendarEventSetNewStatusEpic$ } from './calendar.epics';
import { calendarEventsReducer, CalendarEventsState } from './calendar-events.reducer';
import { EventDialogState, eventDialogReducer } from './event-dialog/event-dialog.reducer';
import { openEventDialogEpic$, closeEventDialogEpic$, startEventDialogProgressEpic$ } from './event-dialog/event-dialog.epics';
import { sickLeaveSavedEpic$, sickLeaveCompletedEpic$, sickLeaveProlongedEpic$, sickLeaveCanceledEpic$ } from './sick-leave.epics';
import { vacationSavedEpic$, vacationCanceledEpic$, vacationChangedEpic$ } from './vacation.epics';
import { dayoffSavedEpic$, dayoffCanceledEpic$ } from './dayoff.epics';
import { loadPendingRequestsEpic$ } from './pending-requests/pending-requests.epics';
import { pendingRequestsReducer, PendingRequestsState } from './pending-requests/pending-requests.reducer';

export interface CalendarState {
    calendarEvents: CalendarEventsState;
    eventDialog: EventDialogState;
    pendingRequests: PendingRequestsState;
}

export const calendarEpics = combineEpics(
    loadUserEmployeeFinishedEpic$ as any,
    loadCalendarEventsEpic$ as any,
    loadCalendarEventsFinishedEpic$ as any,
    loadPendingRequestsEpic$ as any,
    calendarEventSetNewStatusEpic$ as any,
    sickLeaveSavedEpic$ as any,
    sickLeaveCompletedEpic$ as any,
    sickLeaveProlongedEpic$ as any,
    sickLeaveCanceledEpic$ as any,
    vacationSavedEpic$ as any,
    vacationCanceledEpic$ as any,
    vacationChangedEpic$ as any,
    dayoffSavedEpic$ as any,
    dayoffCanceledEpic$ as any,
    intervalsBySingleDaySelectionEpic$ as any,
    openEventDialogEpic$ as any,
    closeEventDialogEpic$ as any,
    startEventDialogProgressEpic$ as any,
    calendarSelectionModeEpic$ as any);

export const calendarReducer = combineReducers<CalendarState>({
    calendarEvents: calendarEventsReducer,
    eventDialog: eventDialogReducer,
    pendingRequests: pendingRequestsReducer
});
