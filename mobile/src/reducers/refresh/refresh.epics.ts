import { ActionsObservable, ofType } from 'redux-observable';
import { Observable } from 'rxjs/Observable';
import { DependenciesContainer, AppState } from '../app.reducer';
import { loadFailedError } from '../errors/errors.action';
import { loadUser } from '../user/user.action';
import { loadDepartments } from '../organization/organization.action';
import { Refresh } from './refresh.action';

export const refreshEpic$ = (action$: ActionsObservable<Refresh>) =>
    action$.ofType('REFRESH')
    .flatMap(x => Observable.of(loadUser()));