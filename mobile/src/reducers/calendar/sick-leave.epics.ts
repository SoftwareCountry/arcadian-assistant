import { ActionsObservable } from 'redux-observable';
import { CompleteSickLeave, ConfirmClaimSickLeave } from './sick-leave.action';
import { AppState, DependenciesContainer } from '../app.reducer';
import { CalendarEventStatus, CalendarEvents, CalendarEventsType } from './calendar-events.model';
import { deserialize } from 'santee-dcts';
import { calendarEventCreated } from './calendar.action';
import { loadFailedError } from '../errors/errors.action';
import { Observable } from 'rxjs/Observable';
import { closeEventDialog } from './event-dialog/event-dialog.action';

export const sickLeaveSavedEpic$ = (action$: ActionsObservable<ConfirmClaimSickLeave>, state: AppState, deps: DependenciesContainer) =>
    action$.ofType('CONFIRM-CLAIM-SICK-LEAVE')
        .flatMap(x => {
            const calendarEvents = new CalendarEvents();

            calendarEvents.type = CalendarEventsType.Sickleave;

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
            ).map(obj => deserialize(obj.response, CalendarEvents));
        })
        .map(x => calendarEventCreated(x))
        .catch((e: Error) => Observable.of(loadFailedError(e.message)));

export const sickLeaveCompletedEpic$ = (action$: ActionsObservable<CompleteSickLeave>, state: AppState, deps: DependenciesContainer) =>
    action$.ofType('COMPLETE-SICK-LEAVE')
        .flatMap(x => {

            x.calendarEvent.status = CalendarEventStatus.Completed;

            return deps.apiClient.put(
                `/employees/${x.employeeId}/events/${x.calendarEvent.calendarEventId}`,
                x.calendarEvent,
                { 'Content-Type': 'application/json' }
            ).map(obj => closeEventDialog());
        });