import { LoadUserEmployeeFinished } from '../user/user.action';
import { ActionsObservable, StateObservable } from 'redux-observable';
import { deserializeArray } from 'santee-dcts';
import {
    loadCalendarEventsFinished, selectIntervalsBySingleDaySelection, SelectCalendarDay,
    LoadCalendarEventsFinished, LoadCalendarEvents, loadCalendarEvents,
    CalendarSelectionMode, disableCalendarSelection, CalendarEventSetNewStatus
} from './calendar.action';
import { loadFailedError } from '../errors/errors.action';
import { CalendarEvent } from './calendar-event.model';
import { closeEventDialog } from './event-dialog/event-dialog.action';
import { AppState } from 'react-native';
import { DependenciesContainer } from '../app.reducer';
import { CalendarEvents } from './calendar-events.model';
import { handleHttpErrors } from '../errors/errors.epics';
import { catchError, flatMap, groupBy, map, mergeAll, mergeMap, switchMap } from 'rxjs/operators';
import { from, Observable, of } from 'rxjs';
import { loadPendingRequests } from './pending-requests/pending-requests.action';
import { Action } from 'redux';

export const loadUserEmployeeFinishedEpic$ = (action$: ActionsObservable<LoadUserEmployeeFinished>, _: StateObservable<AppState>, deps: DependenciesContainer) =>
    action$.ofType('LOAD-USER-EMPLOYEE-FINISHED').pipe(
        map(x => loadCalendarEvents(x.employee.employeeId)),
        catchError((e: Error) => of(loadFailedError(e.message))),
    );

export const loadCalendarEventsFinishedEpic$ = (action$: ActionsObservable<LoadCalendarEventsFinished>, _: StateObservable<AppState>, deps: DependenciesContainer) =>
    action$.ofType('LOAD-CALENDAR-EVENTS-FINISHED').pipe(
        flatMap(action => {
            if (action.next) {
                return from(action.next);
            }
            return of(closeEventDialog());
        }),
    );

export const loadCalendarEventsEpic$ = (action$: ActionsObservable<LoadCalendarEvents>, _: StateObservable<AppState>, deps: DependenciesContainer) =>
    action$.ofType('LOAD-CALENDAR-EVENTS').pipe(
        groupBy(x => x.employeeId),
        map(x =>
            x.pipe(
                switchMap(y =>
                    deps.apiClient.getJSON(`/employees/${x.key}/events`)
                        .pipe(
                            handleHttpErrors(),
                            map(obj => deserializeArray(obj as any, CalendarEvent)),
                            map(calendarEvents => new CalendarEvents(calendarEvents)),
                            map(z => {
                                return { events: z, employeeId: x.key, next: y.next };
                            })
                        )
                ),
            )
        ),
        mergeAll(),
        map(x => loadCalendarEventsFinished(x.events, x.employeeId, x.next)),
    );

export const intervalsBySingleDaySelectionEpic$ = (action$: ActionsObservable<SelectCalendarDay | LoadCalendarEventsFinished>) =>
    action$.ofType(
        'SELECT-CALENDAR-DAY',
        'LOAD-CALENDAR-EVENTS-FINISHED'
    ).pipe(
        map(x => selectIntervalsBySingleDaySelection()),
    );

export const calendarSelectionModeEpic$ = (action$: ActionsObservable<CalendarSelectionMode>) =>
    action$.ofType('CALENDAR-SELECTION-MODE').pipe(
        map(x => disableCalendarSelection(false)),
    );

export const calendarEventSetNewStatusEpic$ = (action$: ActionsObservable<CalendarEventSetNewStatus>, _: StateObservable<AppState>, deps: DependenciesContainer) =>
    action$.ofType('CALENDAR-EVENT-NEW-STATUS').pipe(
        flatMap(x => {
            const requestBody = { ...x.calendarEvent };

            requestBody.status = x.status;

            return deps.apiClient.put(
                `/employees/${x.employeeId}/events/${x.calendarEvent.calendarEventId}`,
                requestBody,
                { 'Content-Type': 'application/json' }
            ).pipe(getEventsAndPendingRequests(x.employeeId));
        }),
        catchError((e: Error) => of(loadFailedError(e.message))),
    );

export function getEventsAndPendingRequests(employeeId: string, next?: Action[]) {
    return <T>(source: Observable<T>) => source.pipe(
        mergeMap(() => from([
            loadCalendarEvents(employeeId, next),
            loadPendingRequests()
        ]))
    );
}
