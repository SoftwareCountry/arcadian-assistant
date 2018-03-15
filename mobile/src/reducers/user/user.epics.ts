import { deserialize } from 'santee-dcts/src/deserializer';
import { loadEmployee, LoadEmployeeFinished } from '../organization/organization.action';
import { ActionsObservable } from 'redux-observable';
import { User } from './user.model';
import { LoadUser, loadUserFinished, LoadUserFinished, loadUserEmployeeFinished, LoadUserEmployeeFinished } from './user.action';
import { Observable } from 'rxjs/Observable';
import { loadFailedError } from '../errors/errors.action';
import { AppState } from 'react-native';
import { DependenciesContainer } from '../app.reducer';

// TODO: Handle error, display some big alert blocking app...
export const loadUserEpic$ = (action$: ActionsObservable<LoadUser>, appState: AppState, deps: DependenciesContainer) =>
    action$.ofType('LOAD-USER')
        .switchMap(x => deps.apiClient.getJSON(`/user`))
        .map(x => deserialize(x, User))
        .map(x => loadUserFinished(x))
        .catch(x => Observable.of(loadFailedError(x.message)));

export const loadUserFinishedEpic$ = (action$: ActionsObservable<LoadUserFinished | LoadEmployeeFinished>) =>
    Observable.combineLatest<LoadUserFinished, LoadEmployeeFinished>(
            action$.ofType('LOAD-USER-FINISHED'), 
            action$.ofType('LOAD_EMPLOYEE_FINISHED'))
        .filter(([userLoaded, employeeLoaded]) => userLoaded.user.employeeId === employeeLoaded.employee.employeeId)
        .first()
        .map(([userLoaded, employeeLoaded]) => loadUserEmployeeFinished(employeeLoaded.employee));
