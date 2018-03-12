import { UserActions, LoadUserEmployeeFinished } from '../user/user.action';
import { ActionsObservable } from 'redux-observable';
import { Observable } from 'rxjs/Observable';
import { ajaxGetJSON } from 'rxjs/observable/dom/AjaxObservable';
import { apiUrl } from '../const';
import { deserializeArray } from 'santee-dcts';
import { loadCalendarEventsFinished } from './calendar.action';
import { loadFailedError } from '../errors/errors.action';
import { CalendarEvents, CalendarEventsType, CalendarEventStatus, DatesInterval } from './calendar-events.model';
import moment from 'moment';

export const loadCalendarEventsFinishedEpic$ = (action$: ActionsObservable<LoadUserEmployeeFinished>) =>
    action$.ofType('LOAD-USER-EMPLOYEE-FINISHED')
        .switchMap(x =>
            ajaxGetJSON(`${apiUrl}/employees/${x.employee.employeeId}/events`).map((obj: Object[]) => deserializeArray(obj, CalendarEvents))
        )
        .map(x => loadCalendarEventsFinished(x))
        .catch((e: Error) => Observable.of(loadFailedError(e.message)));

