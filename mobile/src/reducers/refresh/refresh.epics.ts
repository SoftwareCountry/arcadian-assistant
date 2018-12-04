import { ActionsObservable, ofType } from 'redux-observable';
import { DependenciesContainer, AppState } from '../app.reducer';
import { loadFailedError } from '../errors/errors.action';
import { loadUser } from '../user/user.action';
import { loadDepartments } from '../organization/organization.action';
import { Refresh } from './refresh.action';
import { flatMap } from 'rxjs/operators';
import { of } from 'rxjs';

export const refreshEpic$ = (action$: ActionsObservable<Refresh>) =>
    action$.ofType('REFRESH').pipe(
        flatMap(x => of(loadUser())),
    );
