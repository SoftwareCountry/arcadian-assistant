import { LoadUserEmployeeFinished } from '../user/user.action';
import { ActionsObservable } from 'redux-observable';
import { Observable } from 'rxjs/Observable';
import { ajaxGetJSON, ajaxPost } from 'rxjs/observable/dom/AjaxObservable';
import { apiUrl } from '../const';
import { deserializeArray } from 'santee-dcts';
import { loadCalendarEventsFinished } from './calendar.action';
import { loadFailedError } from '../errors/errors.action';
import { CalendarEvents } from './calendar-events.model';
import { ConfirmClaimSickLeave, sickLeaveSaved, SickLeaveSaved } from './sick-leave.action';

export const loadCalendarEventsFinishedEpic$ = (action$: ActionsObservable<LoadUserEmployeeFinished | SickLeaveSaved>) =>
    action$.filter(x => x.type === 'LOAD-USER-EMPLOYEE-FINISHED' || x.type === 'SICK-LEAVE-SAVED')
        .switchMap(x =>
            ajaxGetJSON(`${apiUrl}/employees/${x.employee.employeeId}/events`).map((obj: Object[]) => deserializeArray(obj, CalendarEvents))
        )
        .map(x => loadCalendarEventsFinished(x))
        .catch((e: Error) => Observable.of(loadFailedError(e.message)));

export const calendarEventsSavedEpic$ = (action$: ActionsObservable<ConfirmClaimSickLeave>) =>
    action$.ofType('CONFIRM-CLAIM-SICK-LEAVE')
        .switchMap(x => {

            return ajaxPost(
                `${apiUrl}/employees/${x.employee.employeeId}/events`,
                x.calendarEvents,
                { 'Content-Type': 'application/json' }).map(() => x.employee);
        })
        .map(x => sickLeaveSaved(x))
        .catch((e: Error) => Observable.of(loadFailedError(e.message)));