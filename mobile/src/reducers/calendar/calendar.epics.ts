/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { LoadUserEmployeeFinished } from '../user/user.action';
import { ActionsObservable, StateObservable } from 'redux-observable';
import { deserializeArray } from 'santee-dcts';
import {
    CalendarEventSetNewStatus,
    CalendarSelectionMode,
    disableCalendarSelection,
    loadCalendarEvents,
    LoadCalendarEvents,
    LoadCalendarEventsFinished,
    loadCalendarEventsFinished,
    SelectCalendarDay,
    selectIntervalsBySingleDaySelection
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
import { loadApprovals } from './approval.action';

//----------------------------------------------------------------------------
export const loadUserEmployeeFinishedEpic$ = (action$: ActionsObservable<LoadUserEmployeeFinished>, _: StateObservable<AppState>, deps: DependenciesContainer) =>
    action$.ofType('LOAD-USER-EMPLOYEE-FINISHED').pipe(
        map(action => loadCalendarEvents(action.employee.employeeId)),
        catchError((e: Error) => of(loadFailedError(e.message))),
    );

//----------------------------------------------------------------------------
export const loadCalendarEventsFinishedEpic$ = (action$: ActionsObservable<LoadCalendarEventsFinished>, _: StateObservable<AppState>, deps: DependenciesContainer) =>
    action$.ofType('LOAD-CALENDAR-EVENTS-FINISHED').pipe(
        flatMap(action => {
            const calendarEventIds = action.calendarEvents.all.map(calendarEvent => calendarEvent.calendarEventId);
            const loadApprovalsAction = loadApprovals(action.employeeId, calendarEventIds);
            if (action.next) {
                return from([...action.next, loadApprovalsAction]);
            }
            return of(closeEventDialog(), loadApprovalsAction);
        }),
    );

//----------------------------------------------------------------------------
export const loadCalendarEventsEpic$ = (action$: ActionsObservable<LoadCalendarEvents>, _: StateObservable<AppState>, deps: DependenciesContainer) =>
    action$.ofType('LOAD-CALENDAR-EVENTS').pipe(
        groupBy(action => action.employeeId),
        map(groupedAction$ =>
            groupedAction$.pipe(
                switchMap(action =>
                    deps.apiClient.getJSON(`/employees/${groupedAction$.key}/events`)
                        .pipe(
                            handleHttpErrors(),
                            map(obj => deserializeArray(obj as any, CalendarEvent)),
                            map(calendarEvents => new CalendarEvents(calendarEvents)),
                            map(calendarEvents => {
                                return { events: calendarEvents, employeeId: groupedAction$.key, next: action.next };
                            })
                        )
                ),
            )
        ),
        mergeAll(),
        map(result => loadCalendarEventsFinished(result.events, result.employeeId, result.next)),
    );

//----------------------------------------------------------------------------
export const intervalsBySingleDaySelectionEpic$ = (action$: ActionsObservable<SelectCalendarDay | LoadCalendarEventsFinished>) =>
    action$.ofType(
        'SELECT-CALENDAR-DAY',
        'LOAD-CALENDAR-EVENTS-FINISHED'
    ).pipe(
        map(action => selectIntervalsBySingleDaySelection()),
    );

//----------------------------------------------------------------------------
export const calendarSelectionModeEpic$ = (action$: ActionsObservable<CalendarSelectionMode>) =>
    action$.ofType('CALENDAR-SELECTION-MODE').pipe(
        map(action => disableCalendarSelection(false)),
    );

//----------------------------------------------------------------------------
export const calendarEventSetNewStatusEpic$ = (action$: ActionsObservable<CalendarEventSetNewStatus>, _: StateObservable<AppState>, deps: DependenciesContainer) =>
    action$.ofType('CALENDAR-EVENT-NEW-STATUS').pipe(
        flatMap(action => {
            const requestBody = { ...action.calendarEvent };

            requestBody.status = action.status;

            return deps.apiClient.put(
                `/employees/${action.employeeId}/events/${action.calendarEvent.calendarEventId}`,
                requestBody,
                { 'Content-Type': 'application/json' }
            ).pipe(getEventsAndPendingRequests(action.employeeId));
        }),
        catchError((e: Error) => of(loadFailedError(e.message))),
    );

//----------------------------------------------------------------------------
export function getEventsAndPendingRequests(employeeId: string, next?: Action[]) {
    return <T>(source: Observable<T>) => source.pipe(
        mergeMap(() => from([
            loadCalendarEvents(employeeId, next),
            loadPendingRequests()
        ]))
    );
}
