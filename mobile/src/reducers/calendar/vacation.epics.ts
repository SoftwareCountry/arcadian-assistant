import { AppState, DependenciesContainer } from '../app.reducer';
import { ConfirmClaimVacation, CancelVacation, ConfirmVacationChange } from './vacation.action';
import { ActionsObservable } from 'redux-observable';
import { CalendarEvent, CalendarEventType, CalendarEventStatus } from './calendar-event.model';
import { deserialize } from 'santee-dcts';
import { loadCalendarEvents } from './calendar.action';
import { Observable } from 'rxjs/Observable';
import { loadFailedError } from '../errors/errors.action';

export const vacationSavedEpic$ = (action$: ActionsObservable<ConfirmClaimVacation>, state: AppState, deps: DependenciesContainer) =>
    action$.ofType('CONFIRM-VACATION')
        .flatMap(x => {
            const calendarEvents = new CalendarEvent();

            calendarEvents.type = CalendarEventType.Vacation;

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
            ).map(obj => deserialize(obj.response, CalendarEvent))
            .map(() => loadCalendarEvents(x.employeeId));
        })
        .catch((e: Error) => Observable.of(loadFailedError(e.message)));

export const vacationCanceledEpic$ = (action$: ActionsObservable<CancelVacation>, state: AppState, deps: DependenciesContainer) =>
    action$.ofType('CANCEL-VACACTION')
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

export const vacationChangedEpic$ = (action$: ActionsObservable<ConfirmVacationChange>, state: AppState, deps: DependenciesContainer) =>
    action$.ofType('CONFIRM-VACATION-CHANGE')
        .flatMap(x => {
            const requestBody = {...x.calendarEvent};

            requestBody.dates = {
                ...requestBody.dates,
                startDate: x.startDate,
                endDate: x.endDate
            };

            return deps.apiClient.put(
                `/employees/${x.employeeId}/events/${x.calendarEvent.calendarEventId}`,
                requestBody,
                { 'Content-Type': 'application/json' }
            ).map(() => loadCalendarEvents(x.employeeId));
        })
        .catch((e: Error) => Observable.of(loadFailedError(e.message)));