import { ActionsObservable } from 'redux-observable';
import { CompleteSickLeave, ConfirmClaimSickLeave, ConfirmProlongSickLeave, CancelSickLeave } from './sick-leave.action';
import { AppState, DependenciesContainer } from '../app.reducer';
import { CalendarEventStatus, CalendarEvent, CalendarEventType } from './calendar-event.model';
import { deserialize } from 'santee-dcts';
import { calendarEventCreated, loadCalendarEvents } from './calendar.action';
import { loadFailedError } from '../errors/errors.action';
import { Observable } from 'rxjs/Observable';
import { closeEventDialog } from './event-dialog/event-dialog.action';

export const sickLeaveSavedEpic$ = (action$: ActionsObservable<ConfirmClaimSickLeave>, state: AppState, deps: DependenciesContainer) =>
    action$.ofType('CONFIRM-CLAIM-SICK-LEAVE')
        .flatMap(x => {
            const calendarEvents = new CalendarEvent();

            calendarEvents.type = CalendarEventType.Sickleave;

            calendarEvents.dates = {
                startDate: x.startDate,
                endDate: x.endDate,
                startWorkingHour: 0,
                finishWorkingHour: 8
            };

            calendarEvents.status = CalendarEventStatus.Requested;

            return deps.apiClient.post(
                `/employees/${x.employeeId}/events`,
                calendarEvents,
                { 'Content-Type': 'application/json' }
            ).map(obj => deserialize(obj.response, CalendarEvent));
        })
        .map(x => calendarEventCreated(x))
        .catch((e: Error) => Observable.of(loadFailedError(e.message)));

export const sickLeaveCompletedEpic$ = (action$: ActionsObservable<CompleteSickLeave>, state: AppState, deps: DependenciesContainer) =>
    action$.ofType('COMPLETE-SICK-LEAVE')
        .flatMap(x => {

            const requestBody = {...x.calendarEvent};

            requestBody.status = CalendarEventStatus.Completed;

            return deps.apiClient.put(
                `/employees/${x.employeeId}/events/${x.calendarEvent.calendarEventId}`,
                requestBody,
                { 'Content-Type': 'application/json' }
            ).map(() => loadCalendarEvents(x.employeeId));
        })
        .catch((e: Error) => Observable.of(loadFailedError(e.message)));

export const sickLeaveProlongedEpic$ = (action$: ActionsObservable<ConfirmProlongSickLeave>, state: AppState, deps: DependenciesContainer) =>
    action$.ofType('CONFIRM-PROLONG-SICK-LEAVE')
        .flatMap(x => {

            const requestBody = {...x.calendarEvent};

            requestBody.dates = {
                ...x.calendarEvent.dates,
                endDate: x.prolongedEndDate
            };

            return deps.apiClient.put(
                `/employees/${x.employeeId}/events/${x.calendarEvent.calendarEventId}`,
                requestBody,
                { 'Content-Type': 'application/json' }
            ).map(() => loadCalendarEvents(x.employeeId));
        })
        .catch((e: Error) => Observable.of(loadFailedError(e.message)));

export const sickLeaveCanceledEpic$ = (action$: ActionsObservable<CancelSickLeave>, state: AppState, deps: DependenciesContainer) =>
    action$.ofType('CANCEL-SICK-LEAVE')
        .flatMap(x => {

            const requestBody = {...x.calendarEvent};

            requestBody.status = CalendarEventStatus.Cancelled;

            return deps.apiClient.put(
                `/employees/${x.employeeId}/events/${x.calendarEvent.calendarEventId}`,
                requestBody,
                { 'Content-Type': 'application/json' }
            ).map(() => loadCalendarEvents(x.employeeId));
        })
        .catch((e: Error) => Observable.of(loadFailedError(e.message)));