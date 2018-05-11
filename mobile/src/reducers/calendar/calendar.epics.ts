import { LoadUserEmployeeFinished } from '../user/user.action';
import { ActionsObservable, ofType } from 'redux-observable';
import { Observable } from 'rxjs/Observable';
import { deserializeArray } from 'santee-dcts';
import {
    loadCalendarEventsFinished, SelectIntervalsBySingleDaySelection, selectIntervalsBySingleDaySelection, SelectCalendarDay, LoadCalendarEventsFinished, LoadCalendarEvents, loadCalendarEvents, 
    CalendarSelectionMode, disableCalendarSelection, DisableCalendarSelection, CalendarSelectionModeType, CalendarEventSetNewStatus
} from './calendar.action';
import { loadFailedError } from '../errors/errors.action';
import { CalendarEvent, CalendarEventStatus, CalendarEventType } from './calendar-event.model';
import { closeEventDialog, CloseEventDialog } from './event-dialog/event-dialog.action';
import { AppState } from 'react-native';
import { DependenciesContainer } from '../app.reducer';
import { CalendarEvents } from './calendar-events.model';

export const loadUserEmployeeFinishedEpic$ = (action$: ActionsObservable<LoadUserEmployeeFinished>, state: AppState, deps: DependenciesContainer) =>
    action$.ofType('LOAD-USER-EMPLOYEE-FINISHED')
        .map(x => loadCalendarEvents(x.employee.employeeId))
        .catch((e: Error) => Observable.of(loadFailedError(e.message)));

export const loadCalendarEventsFinishedEpic$ = (action$: ActionsObservable<LoadCalendarEventsFinished>, state: AppState, deps: DependenciesContainer) =>
    action$.ofType('LOAD-CALENDAR-EVENTS-FINISHED')
        .map(x => closeEventDialog());

export const loadCalendarEventsEpic$ = (action$: ActionsObservable<LoadCalendarEvents>, state: AppState, deps: DependenciesContainer) =>
    action$.ofType('LOAD-CALENDAR-EVENTS')
        .groupBy(x => x.employeeId)
        .map(x =>
            x.switchMap(y =>
                deps.apiClient.getJSON(`/employees/${x.key}/events`)
                    .map(obj => deserializeArray(obj as any, CalendarEvent))
                    .map(calendarEvents => new CalendarEvents(calendarEvents)))
                    .map(z => { 
                        return {events: z, employeeId: x.key};
                    }))
        .mergeAll()
        .map(x => loadCalendarEventsFinished(x.events, x.employeeId))
        .catch((e: Error) => Observable.of(loadFailedError(e.message)));

export const intervalsBySingleDaySelectionEpic$ = (action$: ActionsObservable<SelectCalendarDay | LoadCalendarEventsFinished >) =>
    action$.ofType(
            'SELECT-CALENDAR-DAY',
            'LOAD-CALENDAR-EVENTS-FINISHED'
        ).map(x => selectIntervalsBySingleDaySelection());

export const calendarSelectionModeEpic$ = (action$: ActionsObservable<CalendarSelectionMode>) =>
    action$.ofType('CALENDAR-SELECTION-MODE')
        .map(x => disableCalendarSelection(false));

export const calendarEventSetNewStatusEpic$ = (action$: ActionsObservable<CalendarEventSetNewStatus>, state: AppState, deps: DependenciesContainer) =>
    action$.ofType('CALENDAR-EVENT-NEW-STATUS')
        .flatMap(x => {
            const requestBody = { ...x.calendarEvent };

            requestBody.status = x.status;

            return deps.apiClient.put(
                `/employees/${x.employeeId}/events/${x.calendarEvent.calendarEventId}`,
                requestBody,
                { 'Content-Type': 'application/json' }
            ).map(obj => loadCalendarEvents(x.employeeId));
        })
        .catch((e: Error) => Observable.of(loadFailedError(e.message)));