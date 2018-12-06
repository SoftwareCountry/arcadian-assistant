import { AppState, DependenciesContainer } from '../app.reducer';
import { ConfirmClaimVacation, CancelVacation, ConfirmVacationChange } from './vacation.action';
import { ActionsObservable, StateObservable } from 'redux-observable';
import { CalendarEvent, CalendarEventType, CalendarEventStatus, DatesInterval } from './calendar-event.model';
import { deserialize } from 'santee-dcts';
import { loadCalendarEvents } from './calendar.action';
import { loadFailedError } from '../errors/errors.action';
import { getEventsAndPendingRequests } from './calendar.epics';
import { catchError, flatMap, map } from 'rxjs/operators';
import { of } from 'rxjs';
import { openEventDialog, stopEventDialogProgress } from './event-dialog/event-dialog.action';
import { EventDialogType } from './event-dialog/event-dialog-type.model';

export const vacationSavedEpic$ = (action$: ActionsObservable<ConfirmClaimVacation>, _: StateObservable<AppState>, deps: DependenciesContainer) =>
    action$.ofType('CONFIRM-VACATION').pipe(
        flatMap(x => {
            const calendarEvents = new CalendarEvent();

            calendarEvents.type = CalendarEventType.Vacation;

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
                getEventsAndPendingRequests(x.employeeId, [stopEventDialogProgress(), openEventDialog(EventDialogType.VacationRequested)]),
            );
        }),
        catchError((e: Error) => of(loadFailedError(e.message))),
    );

export const vacationCanceledEpic$ = (action$: ActionsObservable<CancelVacation>, _: StateObservable<AppState>, deps: DependenciesContainer) =>
    action$.ofType('CANCEL-VACACTION').pipe(
        flatMap(x => {
            const requestBody = {...x.calendarEvent};

            requestBody.status = CalendarEventStatus.Cancelled;

            return deps.apiClient.put(
                `/employees/${x.employeeId}/events/${x.calendarEvent.calendarEventId}`,
                requestBody,
                { 'Content-Type': 'application/json' }
            ).pipe(getEventsAndPendingRequests(x.employeeId));
        }),
        catchError((e: Error) => of(loadFailedError(e.message))),
    );

export const vacationChangedEpic$ = (action$: ActionsObservable<ConfirmVacationChange>, _: StateObservable<AppState>, deps: DependenciesContainer) =>
    action$.ofType('CONFIRM-VACATION-CHANGE').pipe(
        flatMap(x => {
            const requestBody = {...x.calendarEvent};

            requestBody.dates = new DatesInterval();
            requestBody.dates.startDate = x.startDate;
            requestBody.dates.endDate = x.endDate;

            return deps.apiClient.put(
                `/employees/${x.employeeId}/events/${x.calendarEvent.calendarEventId}`,
                requestBody,
                { 'Content-Type': 'application/json' }
            ).pipe(getEventsAndPendingRequests(x.employeeId));
        }),
        catchError((e: Error) => of(loadFailedError(e.message))),
    );
