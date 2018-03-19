import { LoadUserEmployeeFinished } from '../user/user.action';
import { ActionsObservable } from 'redux-observable';
import { Observable } from 'rxjs/Observable';
import { deserializeArray, deserialize } from 'santee-dcts';
import { loadCalendarEventsFinished, calendarEventCreated, CalendarEventCreated, IntervalsBySingleDaySelection, CalendarSelectionMode, CalendarSelectionModeType, intervalsBySingleDaySelection, SelectCalendarDay, LoadCalendarEventsFinished } from './calendar.action';
import { loadFailedError } from '../errors/errors.action';
import { CalendarEvents, CalendarEventStatus, CalendarEventsType } from './calendar-events.model';
import { ConfirmClaimSickLeave } from './sick-leave.action';
import { closeEventDialog } from './event-dialog/event-dialog.action';
import { AppState } from 'react-native';
import { DependenciesContainer } from '../app.reducer';

export const loadCalendarEventsFinishedEpic$ = (action$: ActionsObservable<LoadUserEmployeeFinished>, state: AppState, deps: DependenciesContainer) =>
    action$.filter(x => x.type === 'LOAD-USER-EMPLOYEE-FINISHED')
        .switchMap(x =>
            deps.apiClient
                .getJSON(`/employees/${x.employee.employeeId}/events`)
                .map((obj: Object[]) => deserializeArray(obj, CalendarEvents))
        )
        .map(x => loadCalendarEventsFinished(x))
        .catch((e: Error) => Observable.of(loadFailedError(e.message)));

export const calendarEventCreatedEpic$ = (action$: ActionsObservable<CalendarEventCreated>) =>
    action$.ofType('CALENDAR-EVENT-CREATED')
        .map(x => closeEventDialog());

export const calendarEventsSavedEpic$ = (action$: ActionsObservable<ConfirmClaimSickLeave>, state: AppState, deps: DependenciesContainer) =>
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

export const intervalsBySingleDaySelectionEpic$ = (action$: ActionsObservable<SelectCalendarDay | LoadCalendarEventsFinished | CalendarEventCreated>) =>
    action$.filter(x => x.type === 'SELECT-CALENDAR-DAY'
            || x.type === 'LOAD-CALENDAR-EVENTS-FINISHED'
            || x.type === 'CALENDAR-EVENT-CREATED')
        .map(x => intervalsBySingleDaySelection());