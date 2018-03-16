import { LoadUserEmployeeFinished } from '../user/user.action';
import { ActionsObservable } from 'redux-observable';
import { Observable } from 'rxjs/Observable';
import { ajaxGetJSON, ajaxPost } from 'rxjs/observable/dom/AjaxObservable';
import { apiUrl } from '../const';
import { deserializeArray, deserialize } from 'santee-dcts';
import { loadCalendarEventsFinished, calendarEventCreated, CalendarEventCreated } from './calendar.action';
import { loadFailedError } from '../errors/errors.action';
import { CalendarEvents, CalendarEventStatus, CalendarEventsType } from './calendar-events.model';
import { ConfirmClaimSickLeave } from './sick-leave.action';
import { closeEventDialog } from './event-dialog/event-dialog.action';

export const loadCalendarEventsFinishedEpic$ = (action$: ActionsObservable<LoadUserEmployeeFinished>) =>
    action$.filter(x => x.type === 'LOAD-USER-EMPLOYEE-FINISHED')
        .switchMap(x =>
            ajaxGetJSON(`${apiUrl}/employees/${x.employee.employeeId}/events`).map((obj: Object[]) => deserializeArray(obj, CalendarEvents))
        )
        .map(x => loadCalendarEventsFinished(x))
        .catch((e: Error) => Observable.of(loadFailedError(e.message)));

export const calendarEventCreatedEpic$ = (action$: ActionsObservable<CalendarEventCreated>) => 
    action$.ofType('CALENDAR-EVENT-CREATED')
        .map(x => closeEventDialog());

export const calendarEventsSavedEpic$ = (action$: ActionsObservable<ConfirmClaimSickLeave>) =>
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

            return ajaxPost(
                `${apiUrl}/employees/${x.employeeId}/events`, 
                calendarEvents, 
                { 'Content-Type': 'application/json' }
            ).map(obj => deserialize(obj.response, CalendarEvents));
        })
        .map(x => calendarEventCreated(x))
        .catch((e: Error) => Observable.of(loadFailedError(e.message)));