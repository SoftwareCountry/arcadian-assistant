import { combineReducers } from 'redux';
import { combineEpics } from 'redux-observable';
import {
    calendarEventSetNewStatusEpic$,
    calendarSelectionModeEpic$,
    intervalsBySingleDaySelectionEpic$,
    loadCalendarEventsEpic$,
    loadCalendarEventsFinishedEpic$,
    loadUserEmployeeFinishedEpic$
} from './calendar.epics';
import { calendarEventsReducer, CalendarEventsState } from './calendar-events.reducer';
import { eventDialogReducer, EventDialogState } from './event-dialog/event-dialog.reducer';
import {
    closeEventDialogEpic$,
    openEventDialogEpic$,
    startEventDialogProgressEpic$
} from './event-dialog/event-dialog.epics';
import {
    sickLeaveCanceledEpic$,
    sickLeaveCompletedEpic$,
    sickLeaveProlongedEpic$,
    sickLeaveSavedEpic$
} from './sick-leave.epics';
import { vacationCanceledEpic$, vacationChangedEpic$, vacationSavedEpic$ } from './vacation.epics';
import { dayOffCanceledEpic$, dayOffSavedEpic$ } from './dayoff.epics';
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
    dayOffSavedEpic$,
    dayOffCanceledEpic$,
    intervalsBySingleDaySelectionEpic$,
    openEventDialogEpic$,
    closeEventDialogEpic$,
    startEventDialogProgressEpic$,
    calendarSelectionModeEpic$,
    approve$,
    loadApprovals$);

export const calendarReducer = combineReducers<CalendarState | undefined>({
    calendarEvents: calendarEventsReducer,
    eventDialog: eventDialogReducer,
    pendingRequests: pendingRequestsReducer
});
