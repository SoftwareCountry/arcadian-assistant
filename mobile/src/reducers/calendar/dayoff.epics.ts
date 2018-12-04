import { ActionsObservable, StateObservable } from 'redux-observable';
import { ConfirmProcessDayoff, CancelDayoff } from './dayoff.action';
import { AppState, DependenciesContainer } from '../app.reducer';
import { CalendarEvent, CalendarEventType, CalendarEventStatus, DatesInterval } from './calendar-event.model';
import { deserialize } from 'santee-dcts';
import { of } from 'rxjs';
import { loadFailedError } from '../errors/errors.action';
import { IntervalTypeConverter } from './interval-type-converter';
import { getEventsAndPendingRequests } from './calendar.epics';
import { catchError, flatMap, map } from 'rxjs/operators';

export const dayoffSavedEpic$ = (action$: ActionsObservable<ConfirmProcessDayoff>, _: StateObservable<AppState>, deps: DependenciesContainer) =>
    action$.ofType('CONFIRM-PROCESS-DAYOFF').pipe(
        flatMap(x => {
            const calendarEvents = new CalendarEvent();

            calendarEvents.type = x.isWorkout ? CalendarEventType.Workout : CalendarEventType.Dayoff;

            calendarEvents.dates = new DatesInterval();
            calendarEvents.dates.startDate = x.date;
            calendarEvents.dates.endDate = x.date;

            const hours = IntervalTypeConverter.intervalTypeToHours(x.intervalType);

            if (hours) {
                calendarEvents.dates.startWorkingHour = hours.startHour;
                calendarEvents.dates.finishWorkingHour = hours.finishHour;
            }

            calendarEvents.status = CalendarEventStatus.Requested;

            return deps.apiClient.post(
                `/employees/${x.employeeId}/events`,
                calendarEvents,
                { 'Content-Type': 'application/json' }
            ).pipe(
                map(obj => deserialize(obj.response, CalendarEvent)),
            ).pipe(getEventsAndPendingRequests(x.employeeId));
        }),
        catchError((e: Error) => of(loadFailedError(e.message)))
    );

export const dayoffCanceledEpic$ = (action$: ActionsObservable<CancelDayoff>, _: StateObservable<AppState>, deps: DependenciesContainer) =>
    action$.ofType('CANCEL-DAYOFF').pipe(
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
