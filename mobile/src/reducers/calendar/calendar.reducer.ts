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
import { approve$, loadApprovals$ } from './approval.epics';

export interface CalendarState {
    calendarEvents: CalendarEventsState;
    eventDialog: EventDialogState;
    pendingRequests: PendingRequestsState;
}

export const calendarEpics = combineEpics(
    loadUserEmployeeFinishedEpic$,
    loadCalendarEventsEpic$,
    loadCalendarEventsFinishedEpic$,
    loadPendingRequestsEpic$,
    calendarEventSetNewStatusEpic$,
    sickLeaveSavedEpic$,
    sickLeaveCompletedEpic$,
    sickLeaveProlongedEpic$,
    sickLeaveCanceledEpic$,
    vacationSavedEpic$,
    vacationCanceledEpic$,
    vacationChangedEpic$,
    dayoffSavedEpic$,
    dayoffCanceledEpic$,
    intervalsBySingleDaySelectionEpic$,
    openEventDialogEpic$,
    closeEventDialogEpic$,
    startEventDialogProgressEpic$,
    calendarSelectionModeEpic$,
    approve$,
    loadApprovals$);

export const calendarReducer = combineReducers<CalendarState>({
    calendarEvents: calendarEventsReducer,
    eventDialog: eventDialogReducer,
    pendingRequests: pendingRequestsReducer
});
