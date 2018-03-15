import { UserActions, LoadUserEmployeeFinished } from '../user/user.action';
import { ActionsObservable } from 'redux-observable';
import { Observable } from 'rxjs/Observable';
import { deserializeArray } from 'santee-dcts';
import { loadCalendarEventsFinished } from './calendar.action';
import { loadFailedError } from '../errors/errors.action';
import { CalendarEvents, CalendarEventsType, CalendarEventStatus, DatesInterval } from './calendar-events.model';
import moment from 'moment';
import { AppState } from 'react-native';
import { DependenciesContainer } from '../app.reducer';

export const loadCalendarEventsFinishedEpic$ = (action$: ActionsObservable<LoadUserEmployeeFinished>, state: AppState, deps: DependenciesContainer) =>
    action$.ofType('LOAD-USER-EMPLOYEE-FINISHED')
        .switchMap(x =>
            deps.apiClient
                .getJSON(`/employees/${x.employee.employeeId}/events`)
                .map((obj: Object[]) => deserializeArray(obj, CalendarEvents))
        )
        .map(x => loadCalendarEventsFinished(x))
        .catch((e: Error) => Observable.of(loadFailedError(e.message)));

