import { AppState, DependenciesContainer } from '../app.reducer';
import { ConfirmClaimVacation } from './vacation.action';
import { ActionsObservable } from 'redux-observable';
import { CalendarEvent, CalendarEventType, CalendarEventStatus } from './calendar-event.model';
import { deserialize } from 'santee-dcts';
import { calendarEventCreated } from './calendar.action';
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
            ).map(obj => deserialize(obj.response, CalendarEvent));
        })
        .map(x => calendarEventCreated(x))
        .catch((e: Error) => Observable.of(loadFailedError(e.message)));