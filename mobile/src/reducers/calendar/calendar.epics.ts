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
    loadCalendarEventsFailed,
    LoadCalendarEventsFinished,
    loadCalendarEventsFinished,
    SelectCalendarDay,
    selectIntervalsBySingleDaySelection
} from './calendar.action';
import { CalendarEvent } from './calendar-event.model';
import { closeEventDialog } from './event-dialog/event-dialog.action';
import { AppState, DependenciesContainer } from '../app.reducer';
import { CalendarEvents } from './calendar-events.model';
import { handleHttpErrors, handleHttpErrorsWithDefaultValue } from '../../errors/error.operators';
import { flatMap, groupBy, map, mergeAll, mergeMap, switchMap } from 'rxjs/operators';
import { from, Observable, of } from 'rxjs';
import { loadPendingRequests } from './pending-requests/pending-requests.action';
import { Action } from 'redux';
import { loadApprovals } from './approval.action';
import { EmployeeId } from '../organization/employee.model';

//----------------------------------------------------------------------------
export const loadUserEmployeeFinishedEpic$ = (action$: ActionsObservable<LoadUserEmployeeFinished>, _: StateObservable<AppState>, deps: DependenciesContainer) =>
    action$.ofType('LOAD-USER-EMPLOYEE-FINISHED').pipe(
        map(action => loadCalendarEvents(action.employee.employeeId)),
    );

interface CalendarEventsHolder {
    events?: CalendarEvents;
    employeeId: EmployeeId;
    next?: Action[];
    success: boolean;
}

//----------------------------------------------------------------------------
export const loadCalendarEventsEpic$ = (action$: ActionsObservable<LoadCalendarEvents>, _: StateObservable<AppState>, deps: DependenciesContainer) =>
    action$.ofType('LOAD-CALENDAR-EVENTS').pipe(
        groupBy(action => action.employeeId),
        map(groupedAction$ => {
                return groupedAction$.pipe(
                    switchMap(action =>
                        deps.apiClient.getJSON(`/employees/${groupedAction$.key}/events`)
                            .pipe(
                                map(obj => deserializeArray(obj as any, CalendarEvent)),
                                map(calendarEvents => new CalendarEvents(calendarEvents)),
                                map(calendarEvents => {
                                    return {
                                        events: calendarEvents,
                                        employeeId: groupedAction$.key,
                                        next: action.next,
                                        success: true
                                    };
                                }),
                                handleHttpErrorsWithDefaultValue(of({
                                    employeeId: groupedAction$.key,
                                    next: action.next,
                                    success: false
                                })),
                            )
                    ),
                );
            }
        ),
        mergeAll(),
        map((result: CalendarEventsHolder) => {
            if (result.success) {
                return loadCalendarEventsFinished(result.events!, result.employeeId, result.next);
            }

            return loadCalendarEventsFailed(result.employeeId, result.next);
        }),
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
            ).pipe(
                getEventsAndPendingRequests(action.employeeId),
                handleHttpErrors(),
            );
        }),
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
