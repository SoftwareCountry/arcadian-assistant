import { ActionsObservable, ofType } from 'redux-observable';
import { loadUser, LoadUserEmployeeFinished, UserActionType } from '../user/user.action';
import { Refresh } from './refresh.action';
import { flatMap } from 'rxjs/operators';
import { of } from 'rxjs';
import { loadPendingRequests } from '../calendar/pending-requests/pending-requests.action';
import { loadCalendarEvents } from '../calendar/calendar.action';

export const refreshEpic$ = (action$: ActionsObservable<Refresh>) =>
    action$.ofType('REFRESH').pipe(
        flatMap(x => of(loadUser())),
    );

export const refreshUserProfileData$ = (action$: ActionsObservable<LoadUserEmployeeFinished>) => action$.pipe(
    ofType(UserActionType.loadUserEmployeeFinished),
    flatMap(action => of(loadPendingRequests(), loadCalendarEvents(action.employee.employeeId))),
);
