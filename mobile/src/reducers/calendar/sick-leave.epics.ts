import { ActionsObservable, StateObservable } from 'redux-observable';
import {
    CancelSickLeave,
    CompleteSickLeave,
    ConfirmClaimSickLeave,
    ConfirmProlongSickLeave
} from './sick-leave.action';
import { AppState, DependenciesContainer } from '../app.reducer';
import { CalendarEvent, CalendarEventStatus, CalendarEventType, DatesInterval } from './calendar-event.model';
import { deserialize } from 'santee-dcts';
import { getEventsAndPendingRequests } from './calendar.epics';
import { flatMap, map } from 'rxjs/operators';
import { handleHttpErrorsWithDefaultValue } from '../errors/errors.epics';
import { Action } from 'redux';
import { of } from 'rxjs';
import { stopEventDialogProgress } from './event-dialog/event-dialog.action';

export const sickLeaveSavedEpic$ = (action$: ActionsObservable<ConfirmClaimSickLeave>, _: StateObservable<AppState>, deps: DependenciesContainer) =>
    action$.ofType('CONFIRM-CLAIM-SICK-LEAVE').pipe(
        flatMap(x => {
            const calendarEvents = new CalendarEvent();

            calendarEvents.type = CalendarEventType.Sickleave;

            calendarEvents.dates = new DatesInterval();
            calendarEvents.dates.startDate = x.startDate;
            calendarEvents.dates.endDate = x.endDate;
            calendarEvents.dates.startWorkingHour = 0;
            calendarEvents.dates.finishWorkingHour = 8;

            calendarEvents.status = CalendarEventStatus.Requested;

            return deps.apiClient.post(
                `/employees/${x.employeeId}/events`,
                calendarEvents,
                { 'Content-Type': 'application/json' }
            ).pipe(
                map(obj => deserialize(obj.response, CalendarEvent)),
                getEventsAndPendingRequests(x.employeeId),
                handleHttpErrorsWithDefaultValue<Action>(of(stopEventDialogProgress())),
            );
        }),
    );

export const sickLeaveCompletedEpic$ = (action$: ActionsObservable<CompleteSickLeave>, _: StateObservable<AppState>, deps: DependenciesContainer) =>
    action$.ofType('COMPLETE-SICK-LEAVE').pipe(
        flatMap(x => {

            const requestBody = { ...x.calendarEvent };

            requestBody.status = CalendarEventStatus.Completed;

            return deps.apiClient.put(
                `/employees/${x.employeeId}/events/${x.calendarEvent.calendarEventId}`,
                requestBody,
                { 'Content-Type': 'application/json' }
            ).pipe(
                getEventsAndPendingRequests(x.employeeId),
                handleHttpErrorsWithDefaultValue<Action>(of(stopEventDialogProgress())),
            );
        }),
    );

export const sickLeaveProlongedEpic$ = (action$: ActionsObservable<ConfirmProlongSickLeave>, _: StateObservable<AppState>, deps: DependenciesContainer) =>
    action$.ofType('CONFIRM-PROLONG-SICK-LEAVE').pipe(
        flatMap(x => {

            const requestBody = { ...x.calendarEvent };

            requestBody.dates = new DatesInterval();
            requestBody.dates.startDate = x.calendarEvent.dates.startDate;
            requestBody.dates.endDate = x.prolongedEndDate;

            return deps.apiClient.put(
                `/employees/${x.employeeId}/events/${x.calendarEvent.calendarEventId}`,
                requestBody,
                { 'Content-Type': 'application/json' }
            ).pipe(
                getEventsAndPendingRequests(x.employeeId),
                handleHttpErrorsWithDefaultValue<Action>(of(stopEventDialogProgress())),
            );
        }),
    );

export const sickLeaveCanceledEpic$ = (action$: ActionsObservable<CancelSickLeave>, _: StateObservable<AppState>, deps: DependenciesContainer) =>
    action$.ofType('CANCEL-SICK-LEAVE').pipe(
        flatMap(x => {

            const requestBody = { ...x.calendarEvent };

            requestBody.status = CalendarEventStatus.Cancelled;

            return deps.apiClient.put(
                `/employees/${x.employeeId}/events/${x.calendarEvent.calendarEventId}`,
                requestBody,
                { 'Content-Type': 'application/json' }
            ).pipe(
                getEventsAndPendingRequests(x.employeeId),
                handleHttpErrorsWithDefaultValue<Action>(of(stopEventDialogProgress())),
            );
        }),
    );
