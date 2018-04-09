import { ActionsObservable } from 'redux-observable';
import { ConfirmProcessDayoff } from './dayoff.action';
import { AppState, DependenciesContainer } from '../app.reducer';
import { CalendarEvent, CalendarEventType, CalendarEventStatus, DatesInterval } from './calendar-event.model';
import { deserialize } from 'santee-dcts';
import { calendarEventCreated } from './calendar.action';
import { Observable } from 'rxjs';
import { loadFailedError } from '../errors/errors.action';
import { IntervalType } from './calendar.model';
import { IntervalTypeConverter } from './interval-type-converter';

export const dayoffSavedEpic$ = (action$: ActionsObservable<ConfirmProcessDayoff>, state: AppState, deps: DependenciesContainer) =>
    action$.ofType('CONFIRM-PROCESS-DAYOFF')
        .flatMap(x => {
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
            ).map(obj => deserialize(obj.response, CalendarEvent));
        })
        .map(x => calendarEventCreated(x))
        .catch((e: Error) => Observable.of(loadFailedError(e.message)));