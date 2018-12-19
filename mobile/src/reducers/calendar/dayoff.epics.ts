import { ActionsObservable, StateObservable } from 'redux-observable';
import { CancelDayoff, ConfirmProcessDayoff } from './dayoff.action';
import { AppState, DependenciesContainer } from '../app.reducer';
import { CalendarEvent, CalendarEventStatus, CalendarEventType, DatesInterval } from './calendar-event.model';
import { IntervalTypeConverter } from './interval-type-converter';
import { getEventsAndPendingRequests } from './calendar.epics';
import { flatMap } from 'rxjs/operators';
import { openEventDialog, stopEventDialogProgress } from './event-dialog/event-dialog.action';
import { EventDialogType } from './event-dialog/event-dialog-type.model';
import { Action } from 'redux';
import { handleHttpErrorsWithDefaultValue } from '../errors/errors.epics';
import { of } from 'rxjs';

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

            let next: Action[] | undefined;
            if (!x.isWorkout) {
                next = [stopEventDialogProgress(), openEventDialog(EventDialogType.DayoffRequested)];
            }

            return deps.apiClient.post(
                `/employees/${x.employeeId}/events`,
                calendarEvents,
                { 'Content-Type': 'application/json' }
            ).pipe(
                getEventsAndPendingRequests(x.employeeId, next),
                handleHttpErrorsWithDefaultValue<Action>(of(stopEventDialogProgress())),
            );
        }),
    );

export const dayoffCanceledEpic$ = (action$: ActionsObservable<CancelDayoff>, _: StateObservable<AppState>, deps: DependenciesContainer) =>
    action$.ofType('CANCEL-DAYOFF').pipe(
        flatMap(x => {
            const requestBody = { ...x.calendarEvent };

            requestBody.status = CalendarEventStatus.Cancelled;

            return deps.apiClient.put(
                `/employees/${x.employeeId}/events/${x.calendarEvent.calendarEventId}`,
                requestBody,
                { 'Content-Type': 'application/json' }
            ).pipe(
                getEventsAndPendingRequests(x.employeeId),
                handleHttpErrorsWithDefaultValue<Action>(of(stopEventDialogProgress())),
            );
        }),
    );
