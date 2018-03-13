import { LoadUserEmployeeFinished } from '../user/user.action';
import { ActionsObservable } from 'redux-observable';
import { Observable } from 'rxjs/Observable';
import { ajaxGetJSON, ajaxPost } from 'rxjs/observable/dom/AjaxObservable';
import { apiUrl } from '../const';
import { deserializeArray, deserialize } from 'santee-dcts';
import { loadCalendarEventsFinished } from './calendar.action';
import { loadFailedError } from '../errors/errors.action';
import { CalendarEvents } from './calendar-events.model';
import { ConfirmClaimSickLeave } from './sick-leave.action';

export const loadCalendarEventsFinishedEpic$ = (action$: ActionsObservable<LoadUserEmployeeFinished>) =>
    action$.filter(x => x.type === 'LOAD-USER-EMPLOYEE-FINISHED')
        .switchMap(x =>
            ajaxGetJSON(`${apiUrl}/employees/${x.employee.employeeId}/events`).map((obj: Object[]) => deserializeArray(obj, CalendarEvents))
        )
        .map(x => loadCalendarEventsFinished(x))
        .catch((e: Error) => Observable.of(loadFailedError(e.message)));

export const calendarEventsSavedEpic$ = (action$: ActionsObservable<ConfirmClaimSickLeave>) =>
    action$.ofType('CONFIRM-CLAIM-SICK-LEAVE')
        .flatMap(x =>
            ajaxPost(
                `${apiUrl}/employees/${x.employee.employeeId}/events`, 
                x.calendarEvents, 
                { 'Content-Type': 'application/json' }
            )
            .map(obj => deserialize(obj.response, CalendarEvents))
        )
        .map(x => loadCalendarEventsFinished([x]))
        .catch((e: Error) => Observable.of(loadFailedError(e.message)));